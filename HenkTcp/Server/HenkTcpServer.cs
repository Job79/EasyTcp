/* HenkTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

namespace HenkTcp.Server
{
    public class HenkTcpServer
    {
        private ServerListener _ServerListener;

        public event EventHandler<TcpClient> ClientConnected;
        public event EventHandler<TcpClient> ClientDisconnected;
        public event EventHandler<Message> DataReceived;
        public event EventHandler<Exception> OnError;
        public event EventHandler<RefusedClient> OnRefusedConnect;

        /* Every IP in this list will be refused,
         * see ServerListener/_OnClientConnect for details.
         */
        public HashSet<string> BannedIPs = new HashSet<string>();

        public Encoding Encoding = Encoding.UTF8;
        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;

        //Start without encryption.
        public void Start(int Port, int MaxConnections = 10000, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IPAddress.Any, Port, MaxConnections, BufferSize, MaxDataSize);
        public void Start(string IP, int Port, int MaxConnections, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IPAddress.Parse(IP), Port, MaxConnections, null, null, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, int Port, int MaxConnections, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IP, Port, MaxConnections, null, null, BufferSize, MaxDataSize);

        //Start with encryption.
        public void Start(string IP, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IPAddress.Parse(IP), Port, MaxConnections, Password, Salt, Iterations, KeySize, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IP, Port, MaxConnections, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public void Start(string IP, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024, int MaxDataSize = 10240) => Start(IPAddress.Parse(IP), Port, MaxConnections, Algorithm, EncryptionKey, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024, int MaxDataSize = 10240)
        {
            if (_ServerListener != null) return;//Server already running.
            if (Port <= 0 || Port > 65535) throw new Exception("Invalid port number.");
            if (MaxConnections <= 0) throw new Exception("Invalid MaxConnections count.");

            //Create class ServerListener, this will start the passed serverlistener and handle the events.
            _ServerListener = new ServerListener(new TcpListener(IP, Port), this, MaxConnections, BufferSize, MaxDataSize);

            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0) => SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public IEnumerable<TcpClient> ConnectedClients { get { if (_ServerListener == null) return null; return _ServerListener.ConnectedClients; } }
        public int ConnectedClientsCount { get { if (_ServerListener == null) return 0; return _ServerListener.ConnectedClients.Count; } }

        public TcpListener Listener { get { return _ServerListener.Listener; } }
        public bool IsRunning { get { return _ServerListener != null; } }
        public void Stop() { _ServerListener.Listener.Stop(); _ServerListener = null; }
        public void Kick(TcpClient Client) { Client.Client.Shutdown(SocketShutdown.Both); }//Shutdown a connection of a client
        public void Ban(TcpClient Client)
        {
            BannedIPs.Add(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString());//Add client IP to banned IPs
            Kick(Client);//Kick client
        }

        public void Broadcast(string Data) { Broadcast(Encoding.GetBytes(Data)); }
        public void Broadcast(byte[] Data)
        {
            if (_ServerListener == null) return;//Server is not running
            try
            {
                Parallel.ForEach(_ServerListener.ConnectedClients, Client => { //Send every client a message
                    Client.GetStream().WriteAsync(Data, 0, Data.Length);
                });
            }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        public void BroadcastEncrypted(string Data) => BroadcastEncrypted(Encoding.GetBytes(Data));
        public void BroadcastEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null)
            { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set.")); return; }
            Broadcast(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public void Send(TcpClient Client, string Data) => Send(Client, Encoding.GetBytes(Data));
        public void Send(TcpClient Client, byte[] Data)
        {
            if (_ServerListener == null) return;//Server is not running

            try { Client.GetStream().WriteAsync(Data, 0, Data.Length); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        public void SendEncrypted(TcpClient Client, string Data) => SendEncrypted(Client, Encoding.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null)
            { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set.")); return; }
            Send(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public Message SendAndGetReply(TcpClient Client, string Text, TimeSpan Timeout) => SendAndGetReply(Client, Encoding.GetBytes(Text), Timeout);
        public Message SendAndGetReply(TcpClient Client, byte[] Data, TimeSpan Timeout)
        {
            Message Reply = null;
            void Event(object sender, Message e) { Reply = e; DataReceived -= Event; };

            DataReceived += Event;
            Send(Client, Data);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Wait until reply is received from the right client, or time expired.
            while (!(Reply != null && Reply.TcpClient == Client) && sw.Elapsed < Timeout)
                Task.Delay(1).Wait();

            return Reply;
        }

        public Message SendAndGetReplyEncrypted(TcpClient Client, string Text, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, Encoding.GetBytes(Text), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, byte[] Data, TimeSpan Timeout)
        {
            if (_EncryptionKey == null || _Algorithm == null)
            { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set.")); return null; }
            return SendAndGetReply(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);
        }

        /* This functions are used by the ServerListener class. */
        internal void NotifyClientConnected(TcpClient Client) => ClientConnected?.Invoke(this, Client);
        internal void NotifyClientDisconnected(TcpClient Client) => ClientDisconnected?.Invoke(this, Client);
        internal void NotifyDataReceived(byte[] Data, TcpClient Client) => DataReceived?.Invoke(this, new Message(Data, Client, _Algorithm, _EncryptionKey, Encoding));
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
        internal void NotifyOnRefusedConnection(RefusedClient BannedClient) => OnRefusedConnect?.Invoke(this, BannedClient);
    }
}

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
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace HenkTcp.Server
{
    public class HenkTcpServer
    {
        private ServerListener _ServerListener;

        /// <summary>
        /// ClientConnected will be triggerd when a new client connected.
        /// </summary>
        public event EventHandler<TcpClient> ClientConnected;
        /// <summary>
        /// ClientDisconnected will be triggerd when a client disconneced.
        /// </summary>
        public event EventHandler<TcpClient> ClientDisconnected;
        /// <summary>
        /// DataReceived will be triggerd when new data is received.
        /// </summary>
        public event EventHandler<Message> DataReceived;
        /// <summary>
        /// OnError will be triggerd when an error occurs.
        /// </summary>
        public event EventHandler<Exception> OnError;
        /// <summary>
        /// OnRefusedConnect will be triggerd when a client is refused.
        /// Client will be refused when banned or when there are to many clients connected.
        /// </summary>
        public event EventHandler<RefusedClient> OnRefusedConnect;

        /// <summary>
        /// BannedIPs will be used to ban IPs.
        /// BannedIPs contains all the banned IPs.
        /// </summary>
        public HashSet<string> BannedIPs = new HashSet<string>();

        /// <summary>
        /// Encoding will be used to encode string's.
        /// </summary>
        public Encoding Encoding = Encoding.UTF8;

        /// <summary>
        /// Variables that are used for the encryption.
        /// </summary>
        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;

        /// <summary>
        /// Start server without encrypion.
        /// </summary>
        public void Start(ushort Port, int MaxConnections = 10000, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IPAddress.Any, Port, MaxConnections, BufferSize, MaxDataSize);
        public void Start(string IP, ushort Port, int MaxConnections, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IPAddress.Parse(IP), Port, MaxConnections, null, null, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, ushort Port, int MaxConnections, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IP, Port, MaxConnections, null, null, BufferSize, MaxDataSize);

        /// <summary>
        /// Start server with encrypion.
        /// </summary>
        public void Start(string IP, ushort Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IPAddress.Parse(IP), Port, MaxConnections, Password, Salt, Iterations, KeySize, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, ushort Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IP, Port, MaxConnections, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public void Start(string IP, ushort Port, int MaxConnections, SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IPAddress.Parse(IP), Port, MaxConnections, Algorithm, Password, Salt, Iterations, KeySize, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, ushort Port, int MaxConnections, SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IP, Port, MaxConnections, Algorithm, Encryption.CreateKey(Algorithm, Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public void Start(string IP, ushort Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Start(IPAddress.Parse(IP), Port, MaxConnections, Algorithm, EncryptionKey, BufferSize, MaxDataSize);
        public void Start(IPAddress IP, ushort Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, ushort BufferSize = 1024, int MaxDataSize = 10240)
        {
            if (_ServerListener != null) throw new Exception("Server is already running.");
            else if(IP == null) throw new Exception("Invalid IP.");
            else if (Port == 0) throw new Exception("Invalid Port.");
            else if (MaxConnections <= 0) throw new Exception("Invalid MaxConnections count.");
            else if (BufferSize == 0) throw new Exception("Invalid BufferSize.");
            else if (MaxDataSize <= 0) throw new Exception("Invalid MaxDataSize.");

            //Create class ServerListener, this will start the passed serverlistener and handle the events.
            _ServerListener = new ServerListener(new TcpListener(IP, Port), this, MaxConnections, BufferSize, MaxDataSize);

            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        /// <summary>
        /// Set an new EncryptionKey and Algorithm for the encryption.
        /// </summary>
        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0) => SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            if (Algorithm == null) throw new Exception("Algorithm can't be null.");
            else if (EncryptionKey == null) throw new Exception("EncryptionKey can't be null.");

            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        /// <summary>
        /// Return all the ConnectedClients.
        /// </summary>
        public IEnumerable<TcpClient> ConnectedClients { get { if (_ServerListener == null) return null; return _ServerListener.ConnectedClients; } }
        /// <summary>
        /// Return the count of all ConnectedClients.
        /// </summary>
        public int ConnectedClientsCount { get { if (_ServerListener == null) return 0; return _ServerListener.ConnectedClients.Count; } }
        /// <summary>
        /// Return the listener.
        /// </summary>
        public TcpListener Listener { get { if (_ServerListener == null) return null; else return _ServerListener.Listener; } }
        /// <summary>
        /// Return the state of the server.
        /// </summary>
        public bool IsRunning { get { return _ServerListener != null; } }
        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop() { if (_ServerListener != null) { _ServerListener.Listener.Stop(); _ServerListener = null; } }
        /// <summary>
        /// Kick a TcpClient.
        /// </summary>
        public void Kick(TcpClient Client) { Client.Client.Shutdown(SocketShutdown.Both); }//Shutdown a connection of a client
        /// <summary>
        /// Ban a TcpClient.
        /// This will add the client's IP to BannedIPs and kick the client.
        /// </summary>
        public void Ban(TcpClient Client)
        {
            BannedIPs.Add(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString());//Add client IP to banned IPs
            Kick(Client);//Kick client
        }

        /// <summary>
        /// Send a data to all connected client's.
        /// </summary>
        public void BroadcastEncrypted(short Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(int Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(long Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(double Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(float Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(bool Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(char Data) => BroadcastEncrypted(BitConverter.GetBytes(Data));
        public void BroadcastEncrypted(object Data) => BroadcastEncrypted(Serialization.Serialize(Data));
        public void BroadcastEncrypted(string Data) => BroadcastEncrypted(Encoding.GetBytes(Data));
        public void BroadcastEncrypted(byte[] Data) => Broadcast(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));

        public void Broadcast(short Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(int Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(long Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(double Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(float Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(bool Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(char Data) => Broadcast(BitConverter.GetBytes(Data));
        public void Broadcast(object Data) => Broadcast(Serialization.Serialize(Data));
        public void Broadcast(string Data) => Broadcast(Encoding.GetBytes(Data));
        public void Broadcast(byte[] Data)
        {
            if(_ServerListener == null)
            { NotifyOnError(new Exception("Could not send data: Server is not running.")); return; }
            else if (Data == null)
            { NotifyOnError(new Exception("Could not send data: Data is empty.")); return; }

            try
            {
                Parallel.ForEach(_ServerListener.ConnectedClients, Client => { //Send every client a message
                    Client.GetStream().WriteAsync(Data, 0, Data.Length);
                });
            }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        /// <summary>
        /// Send data to 1 TcpClient.
        /// </summary>
        public void SendEncrypted(TcpClient Client, short Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, int Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, long Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, double Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, float Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, bool Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, char Data) => SendEncrypted(Client, BitConverter.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, object Data) => SendEncrypted(Client, Serialization.Serialize(Data));
        public void SendEncrypted(TcpClient Client, string Data) => SendEncrypted(Client, Encoding.GetBytes(Data));
        public void SendEncrypted(TcpClient Client, byte[] Data) => Send(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));

        public void Send(TcpClient Client, short Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, int Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, long Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, double Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, float Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, bool Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, char Data) => Send(Client, BitConverter.GetBytes(Data));
        public void Send(TcpClient Client, object Data) => Send(Client, Serialization.Serialize(Data));
        public void Send(TcpClient Client, string Data) => Send(Client, Encoding.GetBytes(Data));
        public void Send(TcpClient Client, byte[] Data)
        {
            if (_ServerListener == null)
            { NotifyOnError(new Exception("Could not send data: Server is not running.")); return; }
            else if (Data == null)
            { NotifyOnError(new Exception("Could not send data: Data is empty.")); return; }

            try { Client.GetStream().WriteAsync(Data, 0, Data.Length); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        /// <summary>
        /// Sends data to 1 client and wait for a reply from the client.
        /// </summary>
        public Message SendAndGetReplyEncrypted(TcpClient Client, short Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, int Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, long Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, double Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, float Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, bool Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, char Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, object Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, Serialization.Serialize(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, string Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Client, Encoding.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(TcpClient Client, byte[] Data, TimeSpan Timeout) => SendAndGetReply(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);

        public Message SendAndGetReply(TcpClient Client, short Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, int Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, long Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, double Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, float Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, bool Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, char Data, TimeSpan Timeout) => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, object Data, TimeSpan Timeout) => SendAndGetReply(Client, Serialization.Serialize(Data), Timeout);
        public Message SendAndGetReply(TcpClient Client, string Data, TimeSpan Timeout) => SendAndGetReply(Client, Encoding.GetBytes(Data), Timeout);
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

        /* This functions are used by the ServerListener class. */
        internal void NotifyClientConnected(TcpClient Client) => ClientConnected?.Invoke(this, Client);
        internal void NotifyClientDisconnected(TcpClient Client) => ClientDisconnected?.Invoke(this, Client);
        internal void NotifyDataReceived(byte[] Data, TcpClient Client) => DataReceived?.Invoke(this, new Message(Data, Client, _Algorithm, _EncryptionKey, Encoding));
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
        internal void NotifyOnRefusedConnection(RefusedClient BannedClient) => OnRefusedConnect?.Invoke(this, BannedClient);
    }
}

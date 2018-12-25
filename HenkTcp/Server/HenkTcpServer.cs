/* HenkTcp
 * Copyright (C) 2018  henkje (henkje@pm.me)
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

namespace HenkTcp
{
    public class HenkTcpServer
    {
        private ServerListener _ServerListener;

        public event EventHandler<TcpClient> ClientConnected;
        public event EventHandler<TcpClient> ClientDisconnected;
        public event EventHandler<Message> DataReceived;
        public event EventHandler<Exception> OnError;

        public List<string> BannedIps = new List<string>();

        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;

        public void Start(int Port, int MaxConnections = 10000, int BufferSize = 1024) => Start(IPAddress.Any, Port, MaxConnections, BufferSize);
        public void Start(string Ip, int Port, int MaxConnections, int BufferSize = 1024) => Start(IPAddress.Parse(Ip), Port, MaxConnections, null, null, BufferSize);
        public void Start(IPAddress Ip, int Port, int MaxConnections, int BufferSize = 1024) => Start(Ip, Port, MaxConnections, null, null, BufferSize);

        public void Start(string Ip, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024) => Start(IPAddress.Parse(Ip), Port, MaxConnections, Password, Salt, Iterations, KeySize, BufferSize);
        public void Start(IPAddress Ip, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024) => Start(Ip, Port, MaxConnections, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize);
        public void Start(string Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024) => Start(IPAddress.Parse(Ip), Port, MaxConnections, Algorithm, EncryptionKey, BufferSize);
        public void Start(IPAddress Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024)
        {
            if (_ServerListener != null) return;
            if (Port <= 0 || Port > 65535) throw new Exception("Invalid port number");
            if (MaxConnections <= 0) throw new Exception("Invalid MaxConnections count");

            _ServerListener = new ServerListener(new TcpListener(Ip, Port), this, MaxConnections, BufferSize);
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0) => SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public List<TcpClient> ConnectedClients { get { if (_ServerListener == null) return null; return _ServerListener.ConnectedClients; } }
        public int ConnectedClientsCount { get { if (_ServerListener == null) return 0; return _ServerListener.ConnectedClients.Count; } }

        public TcpListener Listener { get { return _ServerListener.Listener; } }
        public bool IsRunning { get { return _ServerListener != null; } }
        public void Stop() { _ServerListener.Listener.Stop(); _ServerListener = null; }

        public void Broadcast(string Data) { Broadcast(Encoding.UTF8.GetBytes(Data)); }
        public void Broadcast(byte[] Data)
        {
            if (_ServerListener == null) return;
            try
            {
                Parallel.ForEach(_ServerListener.ConnectedClients, Client => { Client.GetStream().WriteAsync(Data, 0, Data.Length); });
            }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        public void BroadcastEncrypted(string Data) => BroadcastEncrypted(Encoding.UTF8.GetBytes(Data));
        public void BroadcastEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return; }
            Broadcast(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public void Write(TcpClient Client, string Data) => Write(Client, Encoding.UTF8.GetBytes(Data));
        public void Write(TcpClient Client, byte[] Data)
        {
            try { Client.GetStream().Write(Data, 0, Data.Length); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        public void WriteEncrypted(TcpClient Client, string Data) => WriteEncrypted(Client, Encoding.UTF8.GetBytes(Data));
        public void WriteEncrypted(TcpClient Client, byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return; }
            Write(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public Message WriteAndGetReply(TcpClient Client, string Text, TimeSpan Timeout) => WriteAndGetReply(Client, Encoding.UTF8.GetBytes(Text), Timeout);
        public Message WriteAndGetReply(TcpClient Client, byte[] Data, TimeSpan Timeout)
        {
            Message Reply = null;
            void Event(object sender, Message e) { Reply = e; DataReceived -= Event; };

            DataReceived += Event;
            Write(Client, Data);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            while (!(Reply != null && Reply.TcpClient == Client) && sw.Elapsed < Timeout)
                Task.Delay(1).Wait();

            return Reply;
        }

        public Message WriteAndGetReplyEncrypted(TcpClient Client, string Text, TimeSpan Timeout) => WriteAndGetReplyEncrypted(Client, Encoding.UTF8.GetBytes(Text), Timeout);
        public Message WriteAndGetReplyEncrypted(TcpClient Client, byte[] Data, TimeSpan Timeout)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return null; }
            return WriteAndGetReply(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);
        }

        internal void NotifyClientConnected(TcpClient Client) => ClientConnected?.Invoke(this, Client);
        internal void NotifyClientDisconnected(TcpClient Client) => ClientDisconnected?.Invoke(this, Client);
        internal void NotifyDataReceived(byte[] Data, TcpClient Client) => DataReceived?.Invoke(this, new Message(Data, Client, _Algorithm, _EncryptionKey));
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

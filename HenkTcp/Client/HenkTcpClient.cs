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
using System.Linq;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace HenkTcp.Client
{
    public class HenkTcpClient
    {
        /// <summary>
        /// TcpClient will be used as client.
        /// TcpClient will be null if not connected.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

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
        /// DataReceived will be triggerd when a message is received and no other data is avaible.
        /// </summary>
        public event EventHandler<Message> DataReceived;

        /// <summary>
        /// OnDisconect will be triggerd when the client disconnect's from the server.
        /// </summary>
        public event EventHandler<HenkTcpClient> OnDisconnect;
        /// <summary>
        /// OnError will be triggerd when an error occurs.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// MaxData is used to set a limit on the size of _DataBuffer.
        /// If set to a equal size then the buffer the DataBuffer will be unavaible.
        /// </summary>
        private int _MaxDataSize;

        /// <summary>
        /// Buffer is used for receiving data from the networkstream of TcpClient.
        /// </summary>
        private byte[] _Buffer;

        /// <summary>
        /// DataBuffer will be used when there is more data avaible then fits in the buffer.
        /// </summary>
        private List<byte> _DataBuffer = new List<byte>();

        /// <summary>
        /// Convert string to IPAddress.
        /// Used by the Connect overloads.
        /// </summary>
        private IPAddress _GetIP(string IPString)
        {
            IPAddress IP;
            if (!IPAddress.TryParse(IPString, out IP)) throw new Exception("Invalid IPv4/IPv6 address.");
            return IP;
        }

        /// <summary>
        /// Connect without encryption.
        /// </summary>
        public bool Connect(string IP, ushort Port, TimeSpan Timeout, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(_GetIP(IP), Port, Timeout, null, null, BufferSize, MaxDataSize);
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(IP, Port, Timeout, null, null, BufferSize, MaxDataSize);
        /// <summary>
        /// Connect with encryption.
        /// </summary>
        public bool Connect(string IP, ushort Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(_GetIP(IP), Port, Timeout, Algorithm, Encryption.CreateKey(Algorithm, Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(IP, Port, Timeout, Algorithm, Encryption.CreateKey(Algorithm, Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);

        public bool Connect(string IP, ushort Port, TimeSpan Timeout, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(_GetIP(IP), Port, Timeout, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(IP, Port, Timeout, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);

        public bool Connect(string IP, ushort Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, ushort BufferSize = 1024, int MaxDataSize = 10240)
            => Connect(_GetIP(IP), Port, Timeout, Algorithm, EncryptionKey, BufferSize, MaxDataSize);
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, ushort BufferSize = 1024, int MaxDataSize = 10240)
        {
            if (IP == null) throw new Exception("Invalid IP.");
            else if (Port == 0) throw new Exception("Invalid Port.");
            else if (Timeout.Ticks.Equals(0)) throw new Exception("Invalid Timeout.");
            else if (BufferSize == 0) throw new Exception("Invalid BufferSize.");
            else if (MaxDataSize < BufferSize) throw new Exception("Invalid MaxDataSize.");

            TcpClient = new TcpClient(IP.AddressFamily);
            TcpClient.ConnectAsync(IP, Port);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Wait until tcpclient is connected, or time expired.
            while (!TcpClient.Client.Connected && sw.Elapsed < Timeout)
                Task.Delay(5).Wait();

            if (TcpClient.Connected)
            {
                //Test if connection is realy established. Else client can be banned/rufused/wrong connected, return false.
                if (TcpClient.Client.Poll(0, SelectMode.SelectRead) && TcpClient.Available.Equals(0)) { TcpClient = null; return false; }

                _Algorithm = Algorithm;
                _EncryptionKey = EncryptionKey;
                _MaxDataSize = MaxDataSize;

                _Buffer = new byte[BufferSize];
                //Start listerning for data.
                TcpClient.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, TcpClient);
                return true;
            }
            else { TcpClient = null; return false; }
        }

        /// <summary>
        /// Set an new EncryptionKey and Algorithm for the encryption.
        /// </summary>
        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => SetEncryption(Algorithm, Encryption.CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            if (Algorithm == null) throw new Exception("Algorithm can't be null.");
            else if (EncryptionKey == null) throw new Exception("EncryptionKey can't be null.");

            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        /// <summary>
        /// Close connection of TcpClient.
        /// </summary>
        public void Disconnect(bool NotifyOnDisconnect = false)
        {
            if (TcpClient == null) return;//Client is not connected.

            try
            {
                TcpClient.Client.Shutdown(SocketShutdown.Both);//Shudown connection.
                TcpClient = null;//Set client to null.
            }
            catch (Exception ex) { _NotifyOnError(ex); }

            if (NotifyOnDisconnect) OnDisconnect?.Invoke(this, this);//Call OnDisconnect.
        }

        /// <summary>
        /// Return the connection state of TcpClient.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (TcpClient == null) return false;
                else if (TcpClient.Client.Poll(0, SelectMode.SelectRead) && TcpClient.Available.Equals(0)) { Disconnect(true); return false; }
                else return true;
            }
        }

        /// <summary>
        /// Send data to server.
        /// </summary>
        public void SendEncrypted(short Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(int Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(long Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(double Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(float Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(bool Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(char Data) => SendEncrypted(BitConverter.GetBytes(Data));
        public void SendEncrypted(object Data) => SendEncrypted(Serialization.Serialize(Data));
        public void SendEncrypted(string Data) => SendEncrypted(Encoding.GetBytes(Data));
        public void SendEncrypted(byte[] Data) => Send(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));

        public void Send(short Data) => Send(BitConverter.GetBytes(Data));
        public void Send(int Data) => Send(BitConverter.GetBytes(Data));
        public void Send(long Data) => Send(BitConverter.GetBytes(Data));
        public void Send(double Data) => Send(BitConverter.GetBytes(Data));
        public void Send(float Data) => Send(BitConverter.GetBytes(Data));
        public void Send(bool Data) => Send(BitConverter.GetBytes(Data));
        public void Send(char Data) => Send(BitConverter.GetBytes(Data));
        public void Send(object Data) => Send(Serialization.Serialize(Data));
        public void Send(string Data) => Send(Encoding.GetBytes(Data));
        public void Send(byte[] Data)
        {
            if (TcpClient == null)
            { _NotifyOnError(new Exception("Could not send data: TcpClient not connected.")); return; }
            else if (TcpClient == null)
            { _NotifyOnError(new Exception("Could not send data: Data is empty.")); return; }

            try { TcpClient.GetStream().Write(Data, 0, Data.Length); }//Write async so it won't block UI applications.
            catch (Exception ex) { OnError(this, ex); }
        }

        /// <summary>
        /// Sends data and wait for a reply from the server.
        /// </summary>
        public Message SendAndGetReplyEncrypted(short Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(int Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(long Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(double Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(float Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(bool Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(char Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(object Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Serialization.Serialize(Data), Timeout);
        public Message SendAndGetReplyEncrypted(string Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(Encoding.GetBytes(Data), Timeout);
        public Message SendAndGetReplyEncrypted(byte[] Data, TimeSpan Timeout) => SendAndGetReply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);

        public Message SendAndGetReply(short Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(int Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(long Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(double Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(float Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(bool Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(char Data, TimeSpan Timeout) => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        public Message SendAndGetReply(object Data, TimeSpan Timeout) => SendAndGetReply(Serialization.Serialize(Data), Timeout);
        public Message SendAndGetReply(string Data, TimeSpan Timeout) => SendAndGetReply(Encoding.GetBytes(Data), Timeout);
        public Message SendAndGetReply(byte[] Data, TimeSpan Timeout)
        {
            if (Timeout.Ticks.Equals(0)) throw new Exception("Invalid Timeout.");

            Message Reply = null;
            void Event(object sender, Message e) { Reply = e; DataReceived -= Event; };

            DataReceived += Event;
            Send(Data);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Wait until data is received, or time expired.
            while (Reply == null && sw.Elapsed < Timeout)
                Task.Delay(1).Wait();

            return Reply;
        }


        private void _OnDataReceive(IAsyncResult ar)
        {
            TcpClient Client = ar.AsyncState as TcpClient;//Get tcpclient.

            try
            {
                //Test if client is connected.
                if (Client.Client.Poll(0, SelectMode.SelectRead) && Client.Available.Equals(0))
                { Disconnect(false); OnDisconnect?.Invoke(this, this); return; }

                int ReceivedBytesCount = Client.GetStream().EndRead(ar);
                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Buffer.BlockCopy(_Buffer, 0, ReceivedBytes, 0, ReceivedBytesCount);//Remove null bytes from buffer.

                //When message is longer then the buffer can hold and it is not bigger then MaxDataSize
                if (Client.Available > 0 && Client.Available + _DataBuffer.Count <= _MaxDataSize)
                {
                    _DataBuffer.AddRange(ReceivedBytes);
                    Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, Client);
                    return;
                }

                //When DataBuffer is used and no data is avaible next round OR DataBuffer is full.
                if (_DataBuffer.Any())
                {
                    _DataBuffer.AddRange(ReceivedBytes);
                    ReceivedBytes = _DataBuffer.ToArray();
                    _DataBuffer.Clear();
                }

                DataReceived?.Invoke(this, new Message(ReceivedBytes, Client, _Algorithm, _EncryptionKey, Encoding));
                Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, Client);
            }
            catch (SocketException) { Disconnect(false); OnDisconnect?.Invoke(this, this); }
            catch (Exception ex) { _NotifyOnError(ex); }
        }

        private void _NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

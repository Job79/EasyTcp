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
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace HenkTcp.Client
{
    public class HenkTcpClient
    {
        public TcpClient TcpClient { get; private set; }

        public Encoding Encoding = Encoding.UTF8;
        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;
        private int _MaxDataSize;

        public event EventHandler<Message> DataReceived;
        public event EventHandler<HenkTcpClient> OnDisconnect;
        public event EventHandler<Exception> OnError;

        //Buffer will be used for receiving data.
        private byte[] _Buffer;

        //DataBuffer will be used when client sends a to big message for the buffer.
        private List<byte> _DataBuffer = new List<byte>();

        //Connect without encryption.
        public bool Connect(string IP, int Port, TimeSpan Timeout, int BufferSize = 1024, int MaxDataSize = 10240) => Connect(IP, Port, Timeout, null, null, BufferSize, MaxDataSize);

        //Connect with encryption.
        public bool Connect(string IP, int Port, TimeSpan Timeout, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024, int MaxDataSize = 10240) => Connect(IP, Port, Timeout, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize, MaxDataSize);
        public bool Connect(string IP, int Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024, int MaxDataSize = 10240)
        {
            if (string.IsNullOrEmpty(IP)) throw new Exception("Invalid IP.");
            if (Port <= 0 || Port > 65535) throw new Exception("Invalid port number.");

            TcpClient = new TcpClient();
            TcpClient.ConnectAsync(IP, Port);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Wait until tcpclient is connected, or time expired.
            while (!TcpClient.Client.Connected && sw.Elapsed < Timeout)
                Task.Delay(5).Wait();

            if (TcpClient.Connected)
            {
                _Algorithm = Algorithm;
                _EncryptionKey = EncryptionKey;
                _MaxDataSize = MaxDataSize;

                _Buffer = new byte[BufferSize];
                //Start listerning for data.
                TcpClient.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, TcpClient);
                return true;
            }
            else return false;
        }

        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0)=> SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize));
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public void Disconnect(bool NotifyOnDisconnect = false)
        {
            if (TcpClient == null) return;//Client is not connected.
            TcpClient.Client.Shutdown(SocketShutdown.Both);//Shudown connection.
            TcpClient = null;//Set client to null.

            if (NotifyOnDisconnect) OnDisconnect?.Invoke(this, this);//Call OnDisconnect.
        }

        public bool IsConnected { get { return TcpClient != null; } }

        public void Send(string Data) => Send(Encoding.GetBytes(Data));
        public void Send(byte[] Data)
        {
            try { TcpClient.GetStream().WriteAsync(Data, 0, Data.Length); }//Write async so it won't block UI applications.
            catch (Exception ex) { OnError(this, ex); }
        }

        public void SendEncrypted(string Data) => SendEncrypted(Encoding.GetBytes(Data));
        public void SendEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null)
            { _NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set.")); return; }
            Send(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public Message SendAndGetReply(string Text, TimeSpan Timeout) => SendAndGetReply(Encoding.GetBytes(Text), Timeout);
        public Message SendAndGetReply(byte[] Data, TimeSpan Timeout)
        {
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

        public Message SendAndGetReplyEncrypted(string Text, TimeSpan Timeout) => SendAndGetReplyEncrypted(Encoding.GetBytes(Text), Timeout);
        public Message SendAndGetReplyEncrypted(byte[] Data, TimeSpan Timeout)
        {
            if (_EncryptionKey == null || _Algorithm == null)
            { _NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set.")); return null; }
            return SendAndGetReply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);
        }

        private void _OnDataReceive(IAsyncResult ar)
        {
            TcpClient Client = ar.AsyncState as TcpClient;//Get tcpclient.
            if (Client == null) return;

            try
            {
                //Test if client is connected.
                if (Client.Client.Poll(0, SelectMode.SelectRead) && Client.Client.Available.Equals(0))
                { Disconnect(); OnDisconnect?.Invoke(this, this); return; }

                int ReceivedBytesCount = Client.Client.EndReceive(ar);
                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Buffer.BlockCopy(_Buffer, 0, ReceivedBytes, 0, ReceivedBytesCount);//Remove null bytes from buffer.

                if (Client.Available > 0 && Client.Available + _DataBuffer.Count <= _MaxDataSize)//When message is longer then the buffer can hold.
                {
                    _DataBuffer.AddRange(ReceivedBytes);
                    Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, Client);
                    return;
                }

                if (_DataBuffer.Count > 0)//When DataBuffer is used and no data is avaible next round OR DataBuffer is full
                {
                    _DataBuffer.AddRange(ReceivedBytes);
                    ReceivedBytes = _DataBuffer.ToArray();
                    _DataBuffer.Clear();
                }

                Message m = new Message(ReceivedBytes, Client, _Algorithm, _EncryptionKey, Encoding);
                DataReceived?.Invoke(this, m);

                Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, Client);
            }
            catch (SocketException) { Disconnect(); OnDisconnect?.Invoke(this, this); }
            catch (Exception ex) { _NotifyOnError(ex); }
        }

        private void _NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

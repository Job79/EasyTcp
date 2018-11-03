using System;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace HenkTcp
{
    public class HenkTcpClient
    {
        public TcpClient TcpClient { get; private set; }

        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;

        public event EventHandler<Message> DataReceived;
        public event EventHandler<HenkTcpClient> OnDisconnect;
        public event EventHandler<Exception> OnError;

        private byte[] _Buffer;

        public bool Connect(string Ip, int Port, TimeSpan Timeout, int BufferSize = 1024) { return Connect(Ip, Port, Timeout, null, null, BufferSize); }
        public bool Connect(string Ip, int Port, TimeSpan Timeout, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024) { return Connect(Ip, Port, Timeout, Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize), BufferSize); }
        public bool Connect(string Ip, int Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024)
        {
            if (string.IsNullOrEmpty(Ip)) throw new Exception("Invalid ip");
            if (Port <= 0 || Port > 65535) throw new Exception("Invalid port number");

            TcpClient = new TcpClient();
            var x =TcpClient.ConnectAsync(Ip, Port);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!TcpClient.Client.Connected && sw.Elapsed < Timeout)
            {
                Task.Delay(5).Wait();
            }

            if (TcpClient.Connected)
            {
                _Algorithm = Algorithm;
                _EncryptionKey = EncryptionKey;

                _Buffer = new byte[BufferSize];
                TcpClient.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, TcpClient);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0) { SetEncryption(Aes.Create(), Encryption.CreateKey(Aes.Create(), Password, Salt, Iterations, KeySize)); }
        public void SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public void Disconnect(bool NotifyOnDisconnect = false)
        {
            if (TcpClient == null) return;
            TcpClient.Close();
            TcpClient = null;

            if (NotifyOnDisconnect) OnDisconnect?.Invoke(this, this);
        }

        public bool IsConnected { get { return TcpClient != null; } }

        public void Write(string Data) { Write(Encoding.UTF8.GetBytes(Data)); }
        public void Write(byte[] Data)
        {
            try
            {
                TcpClient.GetStream().Write(Data, 0, Data.Length);
            }
            catch (Exception ex) { OnError(this, ex); }
        }

        public void WriteEncrypted(string Data) { WriteEncrypted(Encoding.UTF8.GetBytes(Data)); }
        public void WriteEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return; }
            Write(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public Message WriteAndGetReply(string Text, TimeSpan Timeout) { return WriteAndGetReply(Encoding.UTF8.GetBytes(Text), Timeout); }
        public Message WriteAndGetReply(byte[] Data, TimeSpan Timeout)
        {
            Message Reply = null;
            void Event(object sender, Message e) { Reply = e; DataReceived -= Event; };

            DataReceived += Event;
            Write(Data);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (Reply == null && sw.Elapsed < Timeout)
            {
                Task.Delay(1).Wait();
            }
            return Reply;
        }

        public Message WriteAndGetReplyEncrypted(string Text, TimeSpan Timeout) { return WriteAndGetReplyEncrypted(Encoding.UTF8.GetBytes(Text), Timeout); }
        public Message WriteAndGetReplyEncrypted(byte[] Data, TimeSpan Timeout)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return null; }
            return WriteAndGetReply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey), Timeout);
        }

        private void _OnDataReceive(IAsyncResult ar)
        {
            TcpClient Client = ar.AsyncState as TcpClient;
            if (Client == null || TcpClient == null) return;

            try
            {
                int ReceivedBytesCount = Client.Client.EndReceive(ar);
                if (ReceivedBytesCount <= 0) { if (Client.Client.Poll(0, SelectMode.SelectRead)) { Disconnect(); OnDisconnect?.Invoke(this, this); } return; }

                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Array.Copy(_Buffer, ReceivedBytes, ReceivedBytes.Length);

                Message m = new Message(ReceivedBytes, Client, _Algorithm, _EncryptionKey);
                DataReceived?.Invoke(this, m);

                TcpClient.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, _OnDataReceive, TcpClient);
            }
            catch (SocketException) { Disconnect(); OnDisconnect?.Invoke(this, this); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

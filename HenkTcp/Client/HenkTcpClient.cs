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

        public byte[] Buffer;

        public bool Connect(string Ip, int Port, TimeSpan Timeout, int BufferSize = 1024) { return Connect(Ip, Port, Timeout, null, null, BufferSize); }
        public bool Connect(string Ip, int Port, TimeSpan Timeout, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024)
        {
            TcpClient = new TcpClient();
            TcpClient.ConnectAsync(Ip, Port);

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

                Buffer = new byte[BufferSize];
                TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, _OnDataReceive, TcpClient);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Disconnect()
        {
            if (TcpClient == null) { return; }
            TcpClient.Dispose();
            TcpClient = null;
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

            DataReceived += (x, r) => { Reply = r; };
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
            if (Client == null) return;

            try
            {
                byte[] ReceivedBytes = new byte[Client.Client.EndReceive(ar)];
                Array.Copy(Buffer, ReceivedBytes, ReceivedBytes.Length);

                Message m = new Message(ReceivedBytes, Client, _Algorithm, _EncryptionKey);
                DataReceived?.Invoke(this, m);

                TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, _OnDataReceive, TcpClient);
            }
            catch (SocketException) { TcpClient.Dispose(); TcpClient = null; OnDisconnect?.Invoke(this,this); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

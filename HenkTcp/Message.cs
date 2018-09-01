using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System;

namespace HenkTcp
{
    public class Message
    {
        private byte[] _Data;
        public TcpClient TcpClient;

        private byte[] _EncryptionKey;
        private SymmetricAlgorithm _Algorithm;

        public Message(byte[] Data, TcpClient Client, SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
        {
            _Data = Data;
            TcpClient = Client;

            _EncryptionKey = EncryptionKey;
            _Algorithm = Algorithm;
        }

        public byte[] Data { get { return _Data; } }
        public byte[] DecryptedData()
        {
            if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
            return Encryption.Decrypt(_Algorithm, _Data, _EncryptionKey);
        }

        public string MessageString { get { return Encoding.UTF8.GetString(Data); } }
        public string DecryptedMessageString { get { return Encoding.UTF8.GetString(DecryptedData()); } }

        public string senderIP { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }

        public void Reply(string data) { Reply(Encoding.UTF8.GetBytes(data)); }
        public void Reply(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void ReplyEncrypted(string Data) { ReplyEncrypted(Encoding.UTF8.GetBytes(Data)); }
        public void ReplyEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
            Reply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }
    }
}

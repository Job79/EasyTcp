using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EasyTcp4.Protocols;

namespace EasyTcp4.Encryption.Ssl
{
    /// <summary>
    /// Protocol that determines the length of a message based on a small header
    /// Header is an int as byte[] with the length of the message.
    /// </summary>
    public class PrefixLengthSslProtocol : SslProtocol
    {
        /// <summary>
        /// Size of the receive buffer 
        /// </summary>
        public override int BufferSize { get; protected set; }

        /// <summary>
        /// Offset of the receive buffer, where to start saving the received data
        /// </summary>
        public override int BufferOffset { get; protected set; }

        /// <summary>
        /// The maximum amount of bytes to receive in the receive buffer
        /// </summary>
        public override int BufferCount
        {
            get => ReceivedHeader ?
                Math.Min(BufferSize - BufferOffset, 10240) : 4; // Do not receive more than 10240 bytes at once
            protected set { }
        }

        /// <summary>
        /// Determines whether the header for the message is received 
        /// </summary>
        protected bool ReceivedHeader;

        /// <summary>
        /// Maximimum amount of bytes for one message
        /// </summary>
        protected readonly int MaxMessageLength;

        /// <summary></summary>
        /// <param name="certificate">server certificate</param>
        /// <param name="maxMessageLength">maximimum amount of bytes for one message</param>
        public PrefixLengthSslProtocol(X509Certificate certificate, int maxMessageLength = ushort.MaxValue) : base(certificate)
        {
            MaxMessageLength = maxMessageLength;
            BufferSize = 4;
            BufferCount = BufferSize;
        }

        /// <summary></summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        /// <param name="maxMessageLength">maximimum amount of bytes for one message</param>
        public PrefixLengthSslProtocol(string serverName, bool acceptInvalidCertificates = false,
                int maxMessageLength = ushort.MaxValue) : base(serverName, acceptInvalidCertificates)
        {
            MaxMessageLength = maxMessageLength;
            BufferSize = 4;
            BufferCount = BufferSize;
        }


        /// <summary>
        /// Send message to remote host
        /// </summary>
        public override void SendMessage(EasyTcpClient client, params byte[][] messageData)
        {
            if (messageData == null || messageData.Length == 0) return;
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: client not connected or null");

            // Calculate length of message
            var messageLength = messageData.Sum(t => t?.Length ?? 0);
            if (messageLength == 0) return;
            if (messageLength > MaxMessageLength)
                throw new ArgumentException("Could not send message: message is too big, increase maxMessageLength or send message with SendArray/SendStream");
            
            // Add header to message 
            int offset = 4;
            byte[] message = new byte[offset + messageLength];
            Buffer.BlockCopy(BitConverter.GetBytes((int)messageLength), 0, message, 0, offset);

            // Add data to message
            foreach (var d in messageData)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, message, offset, d.Length);
                offset += d.Length;
            }


            // Send data
            // Remove prefix in OnDataSend with an offset
            client.FireOnDataSend(message, 4);
            SslStream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Handle received data, this function should trigger the OnDataReceive event of the passed client
        /// </summary>
        /// <param name="data">received data</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client">client that received the data</param>
        public override async Task OnDataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            if (!ReceivedHeader)
            {
                ReceivedHeader = true;
                BufferSize = BitConverter.ToInt32(data, 0);
                if (BufferSize == 0) client.Dispose();
            }
            else
            {
                if (BufferOffset + receivedBytes == BufferSize)
                {
                    ReceivedHeader = false;
                    BufferSize = 4;
                    BufferOffset = 0;
                    await client.DataReceiveHandler(new Message(data, client));
                }
                else BufferOffset += receivedBytes;
            }
        }

        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override IEasyProtocol Clone()
        {
            if (Certificate != null) return new PrefixLengthSslProtocol(Certificate, MaxMessageLength);
            else return new PrefixLengthSslProtocol(ServerName, AcceptInvalidCertificates, MaxMessageLength);
        } 
    }
}

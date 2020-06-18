using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EasyTcp3;

namespace EasyTcp.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// None protocol implementation with ssl
    /// </summary>
    public class NoneSslProtocol : DefaultSslProtocol
    {
        /// <summary>
        /// Default size of the buffer when not specified
        /// </summary>
        private const int DefaultBufferSize = 1024;
        
        /// <summary>
        /// Size of (next) buffer, max size of receiving data
        /// </summary>
        public sealed override int BufferSize { get; protected set; }

        /// <summary>
        /// Constructor if used by a server
        /// </summary>
        /// <param name="certificate">server certificate</param>
        /// <param name="bufferSize"></param>
        public NoneSslProtocol(X509Certificate certificate, int bufferSize = DefaultBufferSize) : base(certificate)
            => BufferSize = bufferSize;

        /// <summary>
        /// Constructor if used by a client
        /// </summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="bufferSize"></param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public NoneSslProtocol(string serverName, int bufferSize = DefaultBufferSize, bool acceptInvalidCertificates = false) : base(serverName, acceptInvalidCertificates)
            => BufferSize = bufferSize; 
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// Example data: [data[] + data1[] + data2[]...]
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception> 
        public override byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0)
                throw new ArgumentException("Could not create message: Data array only contains empty arrays");
            byte[] message = new byte[messageLength];

            // Add data to message
            int offset = 0;
            foreach (var d in data)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, message, offset, d.Length);
                offset += d.Length;
            }

            return message;
        }
        
        /// <summary>
        /// Function that is triggered when new data is received.
        /// </summary>
        /// <param name="data">received data, has the size of the client buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte[] receivedData = new byte[receivedBytes];
            Buffer.BlockCopy(data,0,receivedData,0,receivedBytes); 
            client.DataReceiveHandler(new Message(receivedData, client));
        }
        
        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone()
        {
            if (Certificate != null) return new NoneSslProtocol(Certificate, BufferSize);
            else return new NoneSslProtocol(ServerName, BufferSize, AcceptInvalidCertificates);
        } 
    }
}
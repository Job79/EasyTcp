using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EasyTcp3.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// Protocol that doesn't implements any framing
    /// Useful when communicating with an already existing server/client
    /// </summary>
    public class PlainSslProtocol : DefaultSslProtocol
    {
        /// <summary>
        /// Default size of buffer when not specified in constructor
        /// </summary>
        private const int DefaultBufferSize = 1024;
        
        /// <summary>
        /// Size of (next) buffer used by receive event 
        /// </summary>
        public sealed override int BufferSize { get; protected set; }

        /// <summary>
        /// Constructor for servers
        /// </summary>
        /// <param name="certificate">server certificate</param>
        /// <param name="bufferSize"></param>
        public PlainSslProtocol(X509Certificate certificate, int bufferSize = DefaultBufferSize) : base(certificate)
            => BufferSize = bufferSize;

        /// <summary>
        /// Constructor for clients
        /// </summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="bufferSize"></param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public PlainSslProtocol(string serverName, int bufferSize = DefaultBufferSize, bool acceptInvalidCertificates = false) : base(serverName, acceptInvalidCertificates)
            => BufferSize = bufferSize; 
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// returned data will be send to remote host
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
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
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone()
        {
            if (Certificate != null) return new PlainSslProtocol(Certificate, BufferSize);
            else return new PlainSslProtocol(ServerName, BufferSize, AcceptInvalidCertificates);
        } 
        
        /// <summary>
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override async Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte[] receivedData = new byte[receivedBytes];
            Buffer.BlockCopy(data,0,receivedData,0,receivedBytes); 
            await client.DataReceiveHandler(new Message(receivedData, client));
        }
    }
}
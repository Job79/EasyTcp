using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EasyTcp3;

namespace EasyTcp.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// PrefixLength protocol implementation with ssl
    /// </summary>
    public class PrefixLengthSslProtocol : DefaultSslProtocol
    {
        /// <summary>
        /// Determines whether the next receiving data is the length header or the actual data
        /// </summary>
        protected bool ReceivingLength = true;

        /// <summary>
        /// Size of (next) buffer
        /// 2 when receiving header, else length of receiving data
        /// </summary>
        public sealed override int BufferSize { get; protected set; }
        
        /// <summary>
        /// Constructor if used by a server
        /// </summary>
        /// <param name="certificate">server certificate</param>
        public PrefixLengthSslProtocol(X509Certificate certificate) : base(certificate)
            => BufferSize = 2;

        /// <summary>
        /// Constructor if used by a client
        /// </summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public PrefixLengthSslProtocol(string serverName, bool acceptInvalidCertificates = false) : base(serverName, acceptInvalidCertificates)
            => BufferSize = 2;
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// Example data: [length of data][data[] + data1[] + data2[]...]
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
            byte[] message = new byte[2 + messageLength];

            // Write length of data to message
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) messageLength), 0, message, 0, 2);

            // Add data to message
            int offset = 2;
            foreach (var d in data)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, message, offset, d.Length);
                offset += d.Length;
            }

            return message;
        }

        /// <summary>
        /// Handle received data, trigger event and set new bufferSize determined by the received data 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="receivedBytes">ignored</param>
        /// <param name="client"></param>
        public override void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (ReceivingLength) dataLength = BitConverter.ToUInt16(client.Buffer, 0);
            else client.DataReceiveHandler(new Message(client.Buffer, client));
            ReceivingLength = !ReceivingLength;

            if (dataLength == 0) client.Dispose();
            else BufferSize = dataLength;
        }

        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone()
        {
            if (Certificate != null) return new PrefixLengthSslProtocol(Certificate);
            else return new PrefixLengthSslProtocol(ServerName, AcceptInvalidCertificates);
        } 
    }
}
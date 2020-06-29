using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EasyTcp3.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// Protocol that determines the end of a message based on a sequence of bytes at the end of received data
    /// </summary>
    public class DelimiterSslProtocol : DefaultSslProtocol
    {
        /// <summary>
        /// Sequence of bytes that determines end of receiving message
        /// </summary>
        public readonly byte[] Delimiter;

        /// <summary>
        /// Determines whether the Delimiter gets automatically added to the end of messages when calling Send
        /// </summary>
        protected readonly bool AutoAddDelimiter;

        /// <summary>
        /// Determines whether the Delimiter gets automatically removed from received data
        /// </summary>
        protected readonly bool AutoRemoveDelimiter;

        /// <summary>
        /// List with received bytes
        /// </summary>
        protected readonly List<byte> ReceivedBytes = new List<byte>();

        /// <summary>
        /// BufferSize, always 1 byte
        /// </summary>
        public sealed override int BufferSize { get; protected set; }

        /// <summary>
        /// Constructor for servers
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="certificate">server certificate</param> 
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <exception cref="ArgumentException">delimiter is invalid</exception>
        public DelimiterSslProtocol(byte[] delimiter, X509Certificate certificate, bool autoAddDelimiter = true,
            bool autoRemoveDelimiter = true) : base(certificate)
        {
            if (delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            BufferSize = 1;
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary>
        /// Constructor for servers
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="certificate">server certificate</param> 
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <param name="encoding">encoding used to convert delimiter to byte[]</param>
        /// <exception cref="ArgumentException">delimiter is invalid</exception>
        public DelimiterSslProtocol(string delimiter, X509Certificate certificate,
            bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null) : this(
            (encoding ?? Encoding.UTF8).GetBytes(delimiter), certificate, autoAddDelimiter,
            autoRemoveDelimiter)
        {
        }

        /// <summary>
        /// Constructor for clients
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <exception cref="ArgumentException">delimiter is invalid</exception>
        public DelimiterSslProtocol(byte[] delimiter, string serverName, bool acceptInvalidCertificates = false,
            bool autoAddDelimiter = true, bool autoRemoveDelimiter = true) : base(serverName,
            acceptInvalidCertificates)
        {
            if (delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            BufferSize = 1;
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary>
        /// Constructor for clients
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <param name="encoding">encoding used to convert delimiter to byte[]</param>
        /// <exception cref="ArgumentException">delimiter is invalid</exception>
        public DelimiterSslProtocol(string delimiter, string serverName, bool acceptInvalidCertificates = false,
            bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null) : this(
            (encoding ?? Encoding.UTF8).GetBytes(delimiter), serverName, acceptInvalidCertificates, autoAddDelimiter,
            autoRemoveDelimiter)
        {
        }

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

            byte[] message = new byte[AutoAddDelimiter ? messageLength + Delimiter.Length : messageLength];
            if (AutoAddDelimiter) Buffer.BlockCopy(Delimiter, 0, message, messageLength, Delimiter.Length);

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
            if (Certificate != null)
                return new DelimiterSslProtocol(Delimiter, Certificate, AutoAddDelimiter, AutoRemoveDelimiter);
            else
                return new DelimiterSslProtocol(Delimiter, ServerName, AcceptInvalidCertificates, AutoAddDelimiter,
                    AutoRemoveDelimiter);
        }

        /// <summary>
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte receivedByte = data[0]; // Size of buffer is always 1
            ReceivedBytes.Add(receivedByte);

            // Check delimiter
            if (ReceivedBytes.Count < Delimiter.Length) return;

            int receivedBytesLength = ReceivedBytes.Count - Delimiter.Length;
            for (int i = 0; i < Delimiter.Length; i++)
                if (Delimiter[i] != ReceivedBytes[receivedBytesLength + i])
                    return;

            byte[] receivedData = AutoRemoveDelimiter
                ? ReceivedBytes.Take(receivedBytesLength).ToArray() // Remove delimiter from message
                : ReceivedBytes.ToArray();
            client.DataReceiveHandler(new Message(receivedData, client));
            ReceivedBytes.Clear();
        }
    }
}
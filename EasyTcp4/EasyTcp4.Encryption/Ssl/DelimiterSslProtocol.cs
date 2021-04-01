using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EasyTcp4.Protocols;

namespace EasyTcp4.Encryption.Ssl
{
    /// <summary>
    /// Protocol that determines the end of a message based on a sequence of bytes at the end of the received data 
    /// </summary>
    public class DelimiterSslProtocol : SslProtocol
    {
        /// <summary>
        /// Sequence of bytes that determine the end of a message
        /// </summary>
        public readonly byte[] Delimiter;

        /// <summary>
        /// Determines whether the delimiter gets automatically added to the end of a message
        /// </summary>
        protected readonly bool AutoAddDelimiter;

        /// <summary>
        /// Determines whether the delimiter gets automatically removed from a received message 
        /// </summary>
        protected readonly bool AutoRemoveDelimiter;

        /// <summary>
        /// List with received bytes
        /// </summary>
        protected readonly List<byte> ReceivedBytes = new List<byte>();

        /// <summary>
        /// BufferSize, always 1 byte
        /// </summary>
        public sealed override int BufferSize { get => 1; protected set { } }

        /// <summary></summary>
        /// <param name="certificate">server certificate</param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        public DelimiterSslProtocol(X509Certificate certificate, byte[] delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true)
			: base (certificate)
        {
            if (delimiter == null || delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary></summary>
        /// <param name="certificate">server certificate</param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="encoding">encoding (default: UTF8)</param>
        public DelimiterSslProtocol(X509Certificate certificate, string delimiter,
				bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null)
            : this(certificate, (encoding ?? Encoding.UTF8).GetBytes(delimiter), autoAddDelimiter, autoRemoveDelimiter) { }

		/// <summary></summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        public DelimiterSslProtocol(string serverName, byte[] delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, bool acceptInvalidCertificates = false)
			: base (serverName, acceptInvalidCertificates)
        {
            if (delimiter == null || delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary></summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="encoding">encoding (default: UTF8)</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        public DelimiterSslProtocol(string serverName, string delimiter,
				bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null, bool acceptInvalidCertificates = false)
            : this(serverName, (encoding ?? Encoding.UTF8).GetBytes(delimiter), autoAddDelimiter, autoRemoveDelimiter, acceptInvalidCertificates) { }


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

            byte[] message = new byte[AutoAddDelimiter ? messageLength + Delimiter.Length : messageLength];
            if (AutoAddDelimiter) Buffer.BlockCopy(Delimiter, 0, message, messageLength, Delimiter.Length);

            // Add data to message
            int offset = 0;
            foreach (var d in messageData)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, message, offset, d.Length);
                offset += d.Length;
            }

            // Send data
            // Delimiter is automatically removed in OnDataSend because messageLength is only the length of the message
            client.FireOnDataSend(message, 0, messageLength); 
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
            ReceivedBytes.Clear();
            await client.DataReceiveHandler(new Message(receivedData, client));
        }

        /// <summary>
        /// Return new instance of protocol
        /// </summary>
        public override IEasyProtocol Clone()
        {
            if (Certificate != null) return new DelimiterSslProtocol(Certificate, Delimiter, AutoAddDelimiter, AutoRemoveDelimiter);
            else return new DelimiterSslProtocol(ServerName, Delimiter, AutoAddDelimiter, AutoRemoveDelimiter, AcceptInvalidCertificates);
        }

    }
}

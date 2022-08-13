using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTcp4.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the end of a message based on a sequence of bytes at the end of the received data 
    /// </summary>
    public class DelimiterProtocol : TcpProtocol
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
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        public DelimiterProtocol(byte[] delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true)
        {
            if (delimiter == null || delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary></summary>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="encoding">encoding (default: UTF8)</param>
        public DelimiterProtocol(string delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null)
            : this((encoding ?? Encoding.UTF8).GetBytes(delimiter), autoAddDelimiter, autoRemoveDelimiter) { }

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
            client.BaseSocket.Send(message);
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
        public override IEasyProtocol Clone() => new DelimiterProtocol(Delimiter, AutoAddDelimiter, AutoRemoveDelimiter);
    }
}

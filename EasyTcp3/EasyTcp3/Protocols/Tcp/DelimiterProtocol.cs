using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the end of a message based on a sequence of bytes at the end of received data
    /// </summary>
    public class DelimiterProtocol : DefaultTcpProtocol 
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

        /// <summary></summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <exception cref="ArgumentException">delimiter is invalid</exception>
        public DelimiterProtocol(byte[] delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true)
        {
            if (delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            BufferSize = 1;
            Delimiter = delimiter;
            AutoAddDelimiter = autoAddDelimiter;
            AutoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary></summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving message</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from received data</param>
        /// <param name="encoding">encoding used to convert delimiter to byte[]</param>
        public DelimiterProtocol(string delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true,
            Encoding encoding = null) : this(
            (encoding ?? Encoding.UTF8).GetBytes(delimiter), autoAddDelimiter, autoRemoveDelimiter)
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
        public override object Clone() => new DelimiterProtocol(Delimiter, AutoAddDelimiter, AutoRemoveDelimiter);
        
        /// <summary>
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override async Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
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
            await client.DataReceiveHandler(new Message(receivedData, client));
            ReceivedBytes.Clear();
        }
    }
}
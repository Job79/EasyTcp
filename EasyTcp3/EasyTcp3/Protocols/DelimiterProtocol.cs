using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// This protocol receives data byte-by-byte and triggers DataReceived when it encounters a specific delimiter (sequence of bytes)
    /// Example (Delimiter "\r\n"): ["Data\r\n" : "Data"], ["Data", "Data2", "Data3\r\n" : "DataData2Data3"]
    /// This prevents data from merging when sending in a row. Maximum data size is the maximum size of a list
    ///
    /// TODO: Current implementation isn't extremely efficient, maybe there is a better way?
    /// </summary>
    public class DelimiterProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Sequence of bytes that determines end of receiving messages
        /// </summary>
        public readonly byte[] Delimiter;

        /// <summary>
        /// Determines whether the Delimiter gets automatically added to the end of messages when calling Send
        /// </summary>
        private readonly bool _autoAddDelimiter;

        /// <summary>
        /// Determines whether the Delimiter gets automatically removed from receiving data 
        /// </summary>
        private readonly bool _autoRemoveDelimiter;

        /// <summary>
        /// List with received bytes 
        /// </summary>
        private readonly List<byte> _receivedBytes = new List<byte>();
        
        /// <summary>
        /// BufferSize, always 1 byte 
        /// </summary>
        public int BufferSize => 1;

        /// <summary>
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving messages</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from receiving data</param>
        /// <exception cref="ArgumentException"></exception>
        public DelimiterProtocol(byte[] delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true)
        {
            if (delimiter.Length == 0) throw new ArgumentException("Delimiter is invalid");
            Delimiter = delimiter;
            _autoAddDelimiter = autoAddDelimiter;
            _autoRemoveDelimiter = autoRemoveDelimiter;
        }

        /// <summary>
        /// </summary>
        /// <param name="delimiter">sequence of bytes that determines end of receiving messages</param>
        /// <param name="autoAddDelimiter">determines whether the Delimiter gets automatically added to the end of messages when calling Send</param>
        /// <param name="autoRemoveDelimiter">determines whether the Delimiter gets automatically removed from receiving data</param>
        /// <param name="encoding">encoding used when converting delimiter to a byte[]</param>
        public DelimiterProtocol(string delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true,
            Encoding encoding = null) : this(
            (encoding ?? Encoding.UTF8).GetBytes(delimiter), autoAddDelimiter, autoRemoveDelimiter)
        {
        }

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// [data[] + data1[] + data2[]...] [Delimiter if AutoAddDelimiter is true]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with merged data + delimiter: [data] + [delimiter if AutoDelimiter is true]</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception>
        public byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0)
                throw new ArgumentException("Could not create message: Data array only contains empty arrays");

            byte[] message = new byte[_autoAddDelimiter ? messageLength + Delimiter.Length : messageLength];
            if (_autoAddDelimiter) Buffer.BlockCopy(Delimiter, 0, message, messageLength, Delimiter.Length);

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
        public void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte receivedByte = data[0]; // Size of buffer is always 1
            _receivedBytes.Add(receivedByte);

            // Check delimiter
            if (_receivedBytes.Count < Delimiter.Length) return;

            int receivedBytesLength = _receivedBytes.Count - Delimiter.Length;
            for (int i = 0; i < Delimiter.Length; i++)
                if (Delimiter[i] != _receivedBytes[receivedBytesLength + i])
                    return;

            byte[] receivedData = _autoRemoveDelimiter
                ? _receivedBytes.Take(receivedBytesLength).ToArray() // Remove delimiter from message
                : _receivedBytes.ToArray();
            client.DataReceiveHandler(new Message(receivedData, client));
            _receivedBytes.Clear();
        }

        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public object Clone() => new DelimiterProtocol(Delimiter, _autoAddDelimiter, _autoRemoveDelimiter);
    }
}
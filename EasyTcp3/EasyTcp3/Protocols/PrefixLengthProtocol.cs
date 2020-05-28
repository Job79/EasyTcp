using System;
using System.Linq;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// This protocol prefixes data with a ushort. This ushort contains the length of the next receiving data.
    /// Example: [4 : ushort as byte[2]]["data"], [11 : ushort as byte[2]]["exampleData"]
    /// This prevents data from merging when sending in a row. The maximum data size is ushort.MAX 
    /// </summary>
    public class PrefixLengthProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Determines whether the next receiving data is the length of data or actual data. [4 : ushort as byte[2]] ["data"] 
        /// </summary>
        private bool _receivingLength = true;

        /// <summary>
        /// Size of (next) buffer
        /// 2 when receiving data length, {data length} when receiving data
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// </summary>
        public PrefixLengthProtocol() => BufferSize = 2;

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// [length of data[][] : ushort as byte[2]][data[] + data1[] + data2[]...]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with merged data + length: [data length : ushort as byte[2]][data]</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception>
        public byte[] CreateMessage(params byte[][] data)
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
        /// Handle received data, trigger event and set new bufferSize determined by ReceivingData 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="receivedBytes">ignored</param>
        /// <param name="client"></param>
        public void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (_receivingLength) dataLength = BitConverter.ToUInt16(client.Buffer, 0);
            else client.DataReceiveHandler(new Message(client.Buffer, client));
            _receivingLength = !_receivingLength;

            if (dataLength == 0) client.Dispose();
            else BufferSize = dataLength;
        }

        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public object Clone() => new PrefixLengthProtocol();
    }
}
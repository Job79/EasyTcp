using System;
using System.Linq;

namespace EasyTcp3.Protocol
{
    public class EasyTcp2Protocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Determines whether the next receiving data is the length of data or actual data. [Length of data (4)] ["Data"] 
        /// See DataReceive for more information about the protocol.
        /// </summary>
        private bool _receivingData = true;
        
        /// <summary>
        /// Size of (next) buffer
        /// 2 when receiving data length, [data length] when receiving data
        /// </summary>
        public int BufferSize { get; set; }
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// [ushort: length of data][data + data1 + data2...]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with merged data + length: [ushort: data length][data]</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception>
        public byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0) throw new ArgumentException("Could not create message: Data array only contains empty arrays");
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
        /// Handle received data, trigger event and set new buffersize determined by ReceivingData 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="receivedBytes">ignored</param>
        /// <param name="client"></param>
        public void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (_receivingData) client.DataReceiveHandler(new Message(client.Buffer, client));
            else dataLength = BitConverter.ToUInt16(client.Buffer, 0);
            _receivingData ^= true;
            
            if (dataLength == 0) client.Dispose();
            else BufferSize = dataLength;
        }
    }
}
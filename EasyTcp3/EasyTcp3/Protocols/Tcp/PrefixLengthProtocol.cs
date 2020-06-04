using System;
using System.Linq;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the length of a message based on a small header
    /// Header is an ushort as byte[] with length of incoming message
    /// </summary>
    public class PrefixLengthProtocol : DefaultTcpProtocol 
    {
        /// <summary>
        /// Determines whether the next receiving data is the length header or the actual data
        /// </summary>
        protected bool ReceivingLength = true;

        /// <summary>
        /// Size of (next) buffer used by receive event 
        /// </summary>
        public sealed override int BufferSize { get; protected set; }

        /// <summary></summary>
        public PrefixLengthProtocol() => BufferSize = 2;

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
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new PrefixLengthProtocol();
        
        /// <summary>
        /// Handle received data, trigger event and set new bufferSize determined by the header 
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
    }
}
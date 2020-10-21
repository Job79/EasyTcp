using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the length of a message based on a small header
    /// Header is an ushort as byte[] with the length of the incoming message.
    /// </summary>
    public class PrefixLengthProtocol : DefaultTcpProtocol 
    {
        /// <summary>
        /// Determines whether the next receiving data is the length header or the actual data
        /// </summary>
        protected bool ReceivingLength = true;

        /// <summary>
        /// The size of the (next) buffer, used by receive event
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
            if (data == null || data.Length == 0) throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0) throw new ArgumentException("Could not create message: Data array only contains empty arrays");
            if (messageLength > ushort.MaxValue) 
                throw new ArgumentException("Could not create message: Message can't be created & send because it is too big. Send message with the LargeArrayUtil, StreamUtil or use another protocol.");
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
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override async Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            if (!(ReceivingLength = !ReceivingLength))
            {
                BufferSize = BitConverter.ToUInt16(data, 0);
                if (BufferSize == 0) client.Dispose();
            }
            else
            {
                BufferSize = 2;
                await client.DataReceiveHandler(new Message(data, client));
            }
        }
        
        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new PrefixLengthProtocol();
    }
}
using System;
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
        public sealed override int BufferCount { get; protected set; }
        public sealed override int BufferOffset { get; protected set; }
        public const int MaxBufferCount = 1024;

        protected readonly int MaxMessageLength;
        protected readonly bool Extended;

        /// <summary></summary>
        /// <param name="maxMessageLength">TODO</param>
        public PrefixLengthProtocol(int maxMessageLength = ushort.MaxValue)
        {
            MaxMessageLength = maxMessageLength;
            Extended = maxMessageLength > ushort.MaxValue;
            BufferSize = Extended ? 4 : 2;
            BufferCount = BufferSize;
        }
 
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
            if (messageLength > MaxMessageLength) 
                throw new ArgumentException("Could not create message: Message can't be created and send because it is too big. Increaes the MaxMessageLength or send message with the LargeArrayUtil or StreamUtil");
            byte[] message = new byte[(Extended ? 4 : 2) + messageLength];

            // Write length of data to message
            if(Extended) Buffer.BlockCopy(BitConverter.GetBytes((int) messageLength), 0, message, 0, 4);
            else Buffer.BlockCopy(BitConverter.GetBytes((ushort) messageLength), 0, message, 0, 2);

            // Add data to message
            int offset = Extended ? 4 : 2;
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
            if(ReceivingLength)
            {
                BufferSize = Extended ? BitConverter.ToInt32(data, 0) : BitConverter.ToUInt16(data, 0);
                if (BufferSize == 0) client.Dispose();
                BufferCount = Math.Min(BufferSize, MaxBufferCount);
                ReceivingLength = false;
            }
            else
            {
                if(BufferOffset + receivedBytes == BufferSize)
                {
                    BufferSize = Extended ? 4 : 2;
                    BufferOffset = 0;
                    BufferCount = BufferSize;
                    ReceivingLength = true;
                    await client.DataReceiveHandler(new Message(data, client));
                }
                else
                {
                    BufferOffset += receivedBytes;
                    BufferCount = Math.Min(BufferSize - BufferOffset, MaxBufferCount);
                }
            }
        }
        
        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new PrefixLengthProtocol(MaxMessageLength);
    }
}

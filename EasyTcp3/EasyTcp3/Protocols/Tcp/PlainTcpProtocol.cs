using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Protocol that doesn't implements any framing
    /// Useful when communicating with an already existing tcp server/client
    /// </summary>
    public class PlainTcpProtocol : DefaultTcpProtocol 
    {
        /// <summary>
        /// Default size of buffer when not specified in constructor
        /// </summary>
        internal const int DefaultBufferSize = 1024;
        
        /// <summary>
        /// The size of the (next) buffer, used by receive event
        /// </summary>
        public sealed override int BufferSize { get; protected set; }
        
        /// <summary></summary>
        /// <param name="bufferSize">The size of the receive buffer, max length of receiving data</param>
        public PlainTcpProtocol(int bufferSize = DefaultBufferSize) => BufferSize = bufferSize;
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// Returned data will be send to remote host.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public override byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Could not create message: Data array is empty");
            if (data.Length == 1) return data[0]; // No framing needed for this protocol, next steps can be skipped when only 1 array is send

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0) throw new ArgumentException("Could not create message: Data array only contains empty arrays");
            byte[] message = new byte[messageLength];

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
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override async Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte[] receivedData = new byte[receivedBytes];
            Buffer.BlockCopy(data,0,receivedData,0,receivedBytes); 
            await client.DataReceiveHandler(new Message(receivedData, client));
        }
        
        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new PlainTcpProtocol(BufferSize);
    }
}
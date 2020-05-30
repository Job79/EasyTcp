using System;
using System.Linq;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// This is an implementation of NoneProtocol. Could also be named ReceiveWhatIsAvailableAndFitsInTheBufferProtocol. 
    /// It receives {X} amount of bytes every time, and removes bytes that are outside the received bytes count.
    /// Useful when communicating with an already existing server/client.
    /// ! Splits data into multiple events when data is > BufferSize
    /// ! Merges data when calling Send multiple times in a row (only when doing this very fast) 
    /// </summary>
    public class NoneProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Default size of the buffer when not specified.
        /// </summary>
        protected const int DefaultBufferSize = 1024;
        
        /// <summary>
        /// Size of (next) buffer, max size of receiving data.
        /// </summary>
        public int BufferSize { get; protected set; }
        
        /// <summary>
        /// </summary>
        /// <param name="bufferSize"></param>
        public NoneProtocol(int bufferSize = DefaultBufferSize) => this.BufferSize = bufferSize;
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// Merges all data into 1 big array
        /// Example data: [data[] + data1[] + data2[]...]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with merged data</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception> 
        public virtual byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
            if (messageLength == 0)
                throw new ArgumentException("Could not create message: Data array only contains empty arrays");
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
        /// Function that is triggered when new data is received.
        /// </summary>
        /// <param name="data">received data, has the size of the client buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public virtual void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            byte[] receivedData = new byte[receivedBytes];
            Buffer.BlockCopy(data,0,receivedData,0,receivedBytes); 
            client.DataReceiveHandler(new Message(receivedData, client));
        }
        
        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public virtual object Clone() => new NoneProtocol(BufferSize);
    }
}
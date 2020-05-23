using System;
using System.Linq;

namespace EasyTcp3.Protocol
{
    public class DefaultProtocol : IEasyTcpProtocol
    {
        const int DEFAULT_BUFFERSIZE = 1024;
        
        /// <summary>
        /// Size of (next) buffer, max size of receiving data
        /// </summary>
        public int BufferSize { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferSize"></param>
        public DefaultProtocol(int bufferSize = DEFAULT_BUFFERSIZE) => BufferSize = bufferSize;
        
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// [data[] + data1[] + data2[]...]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with merged data</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception> 
        public byte[] CreateMessage(params byte[][] data)
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
    }
}
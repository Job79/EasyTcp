using System;
using System.Linq;
using EasyTcp3.Server;

namespace EasyTcp3.Protocol
{
    public class DefaultProtocol : IEasyTcpProtocol
    {
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

        public int BufferSize { get; set; }
        
        public byte[][] DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            throw new NotImplementedException();
        }

        public bool OnClientConnect(EasyTcpClient client, EasyTcpServer server)
        {
            throw new NotImplementedException();
        }

        public void ReceiveData(byte[] data, EasyTcpClient client)
        {
            throw new NotImplementedException();
        }
    }
}
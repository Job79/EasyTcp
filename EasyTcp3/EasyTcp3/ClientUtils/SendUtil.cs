using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to send information to a remote host
    /// </summary>
    public static class SendUtil
    {
        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// Message: [ushort: length of data][data1]
        /// OR: [ushort: length of data][data + data1 + data2...]
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>byte array with: [ushort: data length][data]</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception>
        internal static byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var messageLength = data.Sum(t => t?.Length ?? 0);
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
        /// Send message to remote host
        /// </summary>
        /// <param name="baseSocket"></param>
        /// <param name="message">[ushort: data length][data + data1 + data2...]</param>
        internal static void SendMessage(Socket baseSocket, byte[] message)
        {
            if (baseSocket == null || !baseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message);
            baseSocket.SendAsync(e);
        }

        /// <summary>
        /// Send data (byte[][]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, params byte[][] data) => SendMessage(client?.BaseSocket, CreateMessage(data));

        /// <summary>
        /// Send data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, byte[] data, bool compression = false)
        {
            if (compression) data = Compression.Compress(data);
            SendMessage(client?.BaseSocket, CreateMessage(data));
        }

        /// <summary>
        /// Send data (ushort) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, ushort data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (short) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, short data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (uint) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, uint data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (int) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, int data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (ulong) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, ulong data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (long) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, long data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (double) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, double data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (bool) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        public static void Send(this EasyTcpClient client, bool data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, string data,
            Encoding encoding = null, bool compression = false)
            => client.Send((encoding ?? Encoding.UTF8).GetBytes(data), compression);
        
        /// <summary>
        /// Send data (IEasyTcpPacket) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, IEasyTcpPacket data, bool compression = false)
            => client.Send(data.ToArray(), compression);
    }
}
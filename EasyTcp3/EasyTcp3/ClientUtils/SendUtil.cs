using System;
using System.Net.Sockets;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to send information to a remote host
    /// </summary>
    public static class SendUtil
    {
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
        public static void Send(this EasyTcpClient client, params byte[][] data) =>
            SendMessage(client?.BaseSocket, client?.Protocol.CreateMessage(data));

        /// <summary>
        /// Send data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            SendMessage(client?.BaseSocket, client?.Protocol.CreateMessage(data));
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
            => client.Send(data.Data, compression);
    }
}
using System;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with all Send functions
    /// </summary>
    public static class SendUtil
    {
        /// <summary>
        /// Send data (byte[][]) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, params byte[][] data) =>
            client.Protocol.SendMessage(client, client.Protocol.CreateMessage(data));

        /// <summary>
        /// Send data (byte[]) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            client.Protocol.SendMessage(client, client.Protocol.CreateMessage(data));
        }

        /// <summary>
        /// Send data (ushort) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, ushort data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (short) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, short data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (uint) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, uint data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (int) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, int data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (ulong) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, ulong data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (long) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, long data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (double) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, double data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (bool) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, bool data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, string data,
            Encoding encoding = null, bool compression = false)
            => client.Send((encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, IEasyTcpPacket data, bool compression = false)
            => client.Send(data.Data, compression);

        /// <summary>
        /// Send data (object) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void Send(this EasyTcpClient client, object data, bool compression = false)
            => client.Send(client?.Serialize(data), compression);
    }
}
using System;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ServerUtils
{
    /// <summary>
    /// Class with SendAll functions
    /// </summary>
    public static class SendAllUtil
    {
        /// <summary>
        /// Send data (byte[][]) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dataArray">data to send to connected clients</param>
        public static void SendAll(this EasyTcpServer server, params byte[][] dataArray)
        {
            if (server == null || !server.IsRunning)
                throw new Exception("Could not send data: Server not running or null");

            var message = server.Protocol.CreateMessage(dataArray);
            foreach (var client in server.GetConnectedClients()) client.Protocol.SendMessage(client, message);
        }

        /// <summary>
        /// Send data (byte[]) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAll(this EasyTcpServer server, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            server.SendAll(dataArray: data);
        }

        /// <summary>
        /// Send data (ushort) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, ushort data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (short) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, short data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (uint) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, uint data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (int) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, int data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (ulong) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, ulong data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (long) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, long data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (double) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, double data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (bool) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        public static void SendAll(this EasyTcpServer server, bool data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null,
            bool compression = false)
            => server.SendAll((encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAll(this EasyTcpServer server, IEasyTcpPacket data, bool compression = false)
            => server.SendAll(data.Data, compression);
        
        /// <summary>
        /// Serialize and send data (object) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAll(this EasyTcpServer server, object data, bool compression = false)
            => server.SendAll(server?.Serialize(data), compression);
    }
}
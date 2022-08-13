using System;
using System.Text;
using EasyTcp4.PacketUtils;

namespace EasyTcp4.ServerUtils
{
    public static class SendAllUtil
    {
        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dataArray">data to send to connected clients</param>
        public static void SendAll(this EasyTcpServer server, params byte[][] dataArray)
        {
            if (server == null || !server.IsRunning) throw new Exception("Could not send data: server not running or null");
            foreach (var client in server.GetConnectedClients()) client.Protocol.SendMessage(client, dataArray);
        }

        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            server.SendAll(dataArray: data);
        }

        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null, bool compression = false)
            => server.SendAll((encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, IEasyPacket data, bool compression = false)
            => server.SendAll(data.Data, compression);

        /// <summary>
        /// Send data to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, object data, bool compression = false)
            => server.SendAll(server?.Serialize(data), compression);
    }
}

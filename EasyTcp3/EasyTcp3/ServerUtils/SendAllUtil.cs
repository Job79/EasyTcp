using System;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ServerUtils
{
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
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            server.SendAll(dataArray: data);
        }

        /// <summary>
        /// Send data (string) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null,
            bool compression = false)
            => server.SendAll((encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, IEasyTcpPacket data, bool compression = false)
            => server.SendAll(data.Data, compression);
        
        /// <summary>
        /// Serialize and send data (object) to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">data to send to the connected clients</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAll(this EasyTcpServer server, object data, bool compression = false)
            => server.SendAll(server?.Serialize(data), compression);
    }
}

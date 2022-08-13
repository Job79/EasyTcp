using System;
using System.Text;
using EasyTcp4.PacketUtils;
using EasyTcp4.ServerUtils;

namespace EasyTcp4.Actions.Utils
{
    public static class SendAllActionUtil
    {
        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, byte[] data = null, bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            server.SendAll(BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, byte[] data = null, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), data, compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="encoding">encoding type (default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, string data, Encoding encoding = null, bool compression = false)
            => server.SendAllAction(action, (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="encoding">encoding type (default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, string data,
            Encoding encoding = null, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, IEasyPacket data, bool compression = false)
            => server.SendAllAction(action, data.Data, compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, IEasyPacket data, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), data.Data, compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, object data, bool compression = false)
            => server.SendAllAction(action, server?.Serialize(data), compression);

        /// <summary>
        /// Send action to connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to connected clients</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, object data, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), server?.Serialize(data), compression);
    }
}

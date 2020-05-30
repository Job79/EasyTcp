using System;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions to send actions with information to all connecting clients
    /// </summary>
    public static class SendAllActionUtil
    {
        /// <summary>
        /// Send action with data (byte[]) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, byte[] data = null, bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            server.SendAll(BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action with data (byte[]) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, byte[] data = null,
            bool compression = false)
            => server.SendAllAction(action.ToActionCode(), data, compression);

        /// <summary>
        /// Send action with data (ushort) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, ushort data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ushort) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, ushort data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, short data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, short data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, uint data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, uint data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, int data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, int data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, ulong data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, ulong data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, long data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, long data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, double data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, double data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, int action, bool data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        public static void SendAllAction(this EasyTcpServer server, string action, bool data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, string data, Encoding encoding = null,
            bool compression = false)
            => server.SendAllAction(action, (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action with data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, string data,
            Encoding encoding = null, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), compression);
        
        /// <summary>
        /// Send action with data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, int action, IEasyTcpPacket data, bool compression = false)
            => server.SendAllAction(action, data.Data, compression);

        /// <summary>
        /// Send action with data (IEasyTcpPacket) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAllAction(this EasyTcpServer server, string action, IEasyTcpPacket data, bool compression = false)
            => server.SendAllAction(action.ToActionCode(), data.Data, compression);
    }
}
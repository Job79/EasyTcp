using System;
using System.Text;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Server.ServerUtils
{
    public static class SendAllUtil
    {
        /// <summary>
        /// Send data (byte[]) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (server == null || !server.IsRunning)
                throw new Exception("Could not send data: Server not running or null");

            foreach (var client in server.GetConnectedClients()) client.Send(data);
        }

        /// <summary>
        /// Send data (ushort) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, ushort data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (short) to all connected clients
        /// </summary>
        /// <param name="server" />
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, short data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (uint) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, uint data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (int) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, int data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (ulong) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, ulong data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (long) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, long data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (double) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, double data) =>
            server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (bool) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, bool data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data">Data to send to all connected clients</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null)
            => server.SendAll((encoding ?? Encoding.UTF8).GetBytes(data));
    }
}
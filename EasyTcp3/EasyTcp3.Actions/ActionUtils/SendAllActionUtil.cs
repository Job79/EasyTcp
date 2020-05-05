using System;
using System.Net.Sockets;
using System.Text;
using EasyTcp3.Server;

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
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (server == null || !server.IsRunning)
                throw new Exception("Could not send data: Server not running or null");

            var message = new byte[2 + 4 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length + 4),
                0, message, 0, 2); // Write length of data to message.
            Buffer.BlockCopy(BitConverter.GetBytes(action),
                0, message, 2, 4); // Write action operationCode to message.
            Buffer.BlockCopy(data, 0, message, 6, data.Length); // Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message);

            foreach (var client in server.GetConnectedClients()) client.BaseSocket.SendAsync(e);
        }

        /// <summary>
        /// Send action with data (byte[]) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, byte[] data)
            => server.SendAllAction(action.ToActionCode(), data);

        /// <summary>
        /// Send action with data (ushort) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, ushort data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ushort) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, ushort data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, short data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, short data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, uint data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, uint data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, int data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, int data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, ulong data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, ulong data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, long data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, long data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, double data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, double data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, bool data) =>
            server.SendAllAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, bool data) =>
            server.SendAllAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, int action, string data, Encoding encoding = null)
            => server.SendAllAction(action, (encoding ?? Encoding.UTF8).GetBytes(data));

        /// <summary>
        /// Send action with data (string) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to all connected clients</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <exception cref="ArgumentException">data array is empty or invalid server</exception>
        public static void SendAllAction(this EasyTcpServer server, string action, string data,
            Encoding encoding = null)
            => server.SendAllAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data));
    }
}
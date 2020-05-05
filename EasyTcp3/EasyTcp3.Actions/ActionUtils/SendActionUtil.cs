using System;
using System.Net.Sockets;
using System.Text;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions to send actions with information to a remote host
    /// </summary>
    public static class SendActionUtil
    {
        /// <summary>
        /// Send action with data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <exception cref="ArgumentException">data array is empty or invalid client</exception>
        public static void SendAction(this EasyTcpClient client, int action, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            var message = new byte[2 + 4 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length + 4),
                0, message, 0, 2); // Write length of data to message.
            Buffer.BlockCopy(BitConverter.GetBytes(action),
                0, message, 2, 4); // Write action operationCode to message.
            Buffer.BlockCopy(data, 0, message, 6, data.Length); // Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message);
            client.BaseSocket.SendAsync(e);
        }

        /// <summary>
        /// Send action with data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        /// <exception cref="ArgumentException">data array is empty or invalid client</exception>
        public static void SendAction(this EasyTcpClient client, string action, byte[] data) =>
            client.SendAction(action.ToActionCode(), data);

        /// <summary>
        /// Send action with data (ushort) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, ushort data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ushort) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, ushort data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, short data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (short) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, short data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, uint data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (uint) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, uint data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, int data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (int) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, int data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, ulong data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (ulong) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, ulong data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, long data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (long) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, long data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, double data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (double) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, double data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, int action, bool data) =>
            client.SendAction(action, BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (bool) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        public static void SendAction(this EasyTcpClient client, string action, bool data) =>
            client.SendAction(action.ToActionCode(), BitConverter.GetBytes(data));

        /// <summary>
        /// Send action with data (string) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        public static void SendAction(this EasyTcpClient client, int action, string data, Encoding encoding = null)
            => client.SendAction(action, (encoding ?? Encoding.UTF8).GetBytes(data));

        /// <summary>
        /// Send action with data (string) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        public static void SendAction(this EasyTcpClient client, string action, string data, Encoding encoding = null)
            => client.SendAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data));
    }
}
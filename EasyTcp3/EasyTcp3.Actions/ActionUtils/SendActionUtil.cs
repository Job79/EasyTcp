using System;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

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
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            client.Send(BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action with data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, byte[] data,
            bool compression = false) =>
            client.SendAction(action.ToActionCode(), data, compression);

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
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, string data, Encoding encoding = null,
            bool compression = false)
            => client.SendAction(action, (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action with data (string) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, string data, Encoding encoding = null,
            bool compression = false)
            => client.SendAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), compression);
    }
}
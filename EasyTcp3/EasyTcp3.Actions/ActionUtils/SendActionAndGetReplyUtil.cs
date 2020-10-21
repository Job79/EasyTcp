using System;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Class with all SendActionAndGetReply functions
    /// </summary>
    public static class SendActionAndGetReplyUtil
    {
        /// <summary>
        /// Send action (byte[]) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns> 
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, byte[] data = null,
            TimeSpan? timeout = null, bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            return client.SendAndGetReply(timeout, BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action (byte[]) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns> 
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, byte[] data = null,
            TimeSpan? timeout = null, bool compression = false) =>
            client.SendActionAndGetReply(action.ToActionCode(), data, timeout, compression);

        /// <summary>
        /// Send action (ushort) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, ushort data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (ushort) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, ushort data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (short) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, short data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (short) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, short data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (uint) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, uint data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (uint) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, uint data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (int) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, int data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (int) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, int data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (ulong) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, ulong data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (ulong) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, ulong data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (long) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, long data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (long) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, long data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (double) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, double data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (double) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, double data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (bool) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, bool data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (bool) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, bool data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action (string) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, string data,
            TimeSpan? timeout = null, Encoding encoding = null, bool compression = false) =>
            client.SendActionAndGetReply(action, (encoding ?? Encoding.UTF8).GetBytes(data), timeout, compression);

        /// <summary>
        /// Send action (string) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, string data,
            TimeSpan? timeout = null, Encoding encoding = null, bool compression = false) =>
            client.SendActionAndGetReply(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), timeout,
                compression);
        
        /// <summary>
        /// Send action (IEasyTcpPacket) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, IEasyTcpPacket data,
            TimeSpan? timeout = null, bool compression = false) =>
            client.SendActionAndGetReply(action, data.Data, timeout, compression);

        /// <summary>
        /// Send action (IEasyTcpPacket) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, IEasyTcpPacket data,
            TimeSpan? timeout = null, bool compression = false) =>
            client.SendActionAndGetReply(action.ToActionCode(), data.Data, timeout, compression);
        
        /// <summary>
        /// Send action (object) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, object data,
            TimeSpan? timeout = null, bool compression = false) =>
            client.SendActionAndGetReply(action, client?.Serialize(data), timeout, compression);

        /// <summary>
        /// Send action (object) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, object data,
            TimeSpan? timeout = null, bool compression = false) =>
            client.SendActionAndGetReply(action.ToActionCode(), client?.Serialize(data), timeout, compression);
    }
}
using System;
using System.Text;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Actions.ActionUtils.Async
{
    /// <summary>
    /// Functions to async send actions to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendActionAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns> 
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, byte[] data = null,
            TimeSpan? timeout = null, bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            return await client.SendAndGetReplyAsync(timeout, BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns> 
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, string action,
            byte[] data = null, TimeSpan? timeout = null, bool compression = false) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), data, timeout, compression);

        /// <summary>
        /// Send action with data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, ushort data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, ushort data,
                TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, short data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, short data,
                TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, uint data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, uint data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, int data,
            TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, string action, int data,
            TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, ulong data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, ulong data,
                TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, long data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, long data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, double data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, double data,
                TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, bool data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, string action, bool data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, string data,
            TimeSpan? timeout = null, Encoding encoding = null, bool compression = false)
            => await client.SendActionAndGetReplyAsync(action, (encoding ?? Encoding.UTF8).GetBytes(data), timeout,
                compression);

        /// <summary>
        /// Send action with data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, string action,
            string data, TimeSpan? timeout = null, Encoding encoding = null, bool compression = false)
            => await client.SendActionAndGetReplyAsync(action.ToActionCode(),
                (encoding ?? Encoding.UTF8).GetBytes(data), timeout, compression);
        
        /// <summary>
        /// Send action with data (IEasyTcpPacket) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, IEasyTcpPacket data,
            TimeSpan? timeout = null, bool compression = false)
            => await client.SendActionAndGetReplyAsync(action, data.Data, timeout, compression);

        /// <summary>
        /// Send action with data (IEasyTcpPacket) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, string action,
            IEasyTcpPacket data,
            TimeSpan? timeout = null, bool compression = false)
            => await client.SendActionAndGetReplyAsync(action.ToActionCode(), data.Data, timeout, compression);
    }
}
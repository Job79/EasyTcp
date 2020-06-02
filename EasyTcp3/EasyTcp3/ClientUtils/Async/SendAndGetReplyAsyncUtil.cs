using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Functions to async send a message to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Default timeout used when no parameter is passed
        /// </summary>
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send data (byte[][]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns> 
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, TimeSpan? timeout = null,
            params byte[][] data)
        {
            Message reply = null;
            using var signal = new SemaphoreSlim(0, 1); // Use SemaphoreSlim as async ManualResetEventSlim

            client.DataReceiveHandler = message =>
            {
                reply = message;
                client.ResetDataReceiveHandler();
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Release();
            };
            client.Send(data);

            await signal.WaitAsync(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.ResetDataReceiveHandler();
            return reply;
        }

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, byte[] data,
            TimeSpan? timeout = null,
            bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            return await client.SendAndGetReplyAsync(timeout, data);
        }


        /// <summary>
        /// Send data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ushort data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, short data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, uint data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, int data,
            TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ulong data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, long data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, double data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, bool data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, string data,
            TimeSpan? timeout = null, Encoding encoding = null, bool compression = false)
            => await client.SendAndGetReplyAsync((encoding ?? Encoding.UTF8).GetBytes(data), timeout, compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, IEasyTcpPacket data,
            TimeSpan? timeout = null, bool compression = false)
            => await client.SendAndGetReplyAsync(data.Data, timeout, compression);
        
        /// <summary>
        /// Send data (object) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, object data,
            TimeSpan? timeout = null, bool compression = false)
            => await client.SendAndGetReplyAsync(client?.Serialize(data), timeout, compression);
    }
}
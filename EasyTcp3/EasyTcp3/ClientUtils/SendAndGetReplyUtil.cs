using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with all SendAndGetReply functions
    /// </summary>
    public static class SendAndGetReplyUtil
    {
        /// <summary>
        /// Default timeout when timeout parameter is not specified 
        /// </summary>
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send data (byte[][]) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, TimeSpan? timeout = null, params byte[][] data)
        {
            Message reply = null;
            using var signal = new ManualResetEventSlim();

            client.DataReceiveHandler = message =>
            {
                reply = message;
                client.ResetDataReceiveHandler();
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Set();
                return Task.CompletedTask;
            };
            client.Send(data);

            signal.Wait(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.ResetDataReceiveHandler();
            return reply;
        }

        /// <summary>
        /// Send data (byte[]) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, byte[] data, TimeSpan? timeout = null,
            bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            return client.SendAndGetReply(timeout, data);
        }

        /// <summary>
        /// Send data (ushort) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, ushort data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, short data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, uint data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, int data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, ulong data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, long data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, double data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, bool data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, string data, TimeSpan? timeout = null,
            Encoding encoding = null, bool compression = false) =>
            client.SendAndGetReply((encoding ?? Encoding.UTF8).GetBytes(data), timeout, compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, IEasyTcpPacket data, TimeSpan? timeout = null,
            bool compression = false) =>
            client.SendAndGetReply(data.Data, timeout, compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to remote host. Then wait and return the reply
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired this function returns null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <returns>received reply</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, object data, TimeSpan? timeout = null,
            bool compression = false) =>
            client.SendAndGetReply(client?.Serialize(data), timeout, compression);
    }
}
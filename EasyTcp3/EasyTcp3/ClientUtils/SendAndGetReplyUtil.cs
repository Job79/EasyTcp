using System;
using System.Text;
using System.Threading;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to send a message to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendAndGetReplyUtil
    {
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send data (byte[][]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
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
            };
            client.Send(data);

            signal.Wait(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
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
        public static Message SendAndGetReply(this EasyTcpClient client, byte[] data, TimeSpan? timeout = null,
            bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            return client.SendAndGetReply(timeout, data);
        }

        /// <summary>
        /// Send data (ushort) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, ushort data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, short data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, uint data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, int data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, ulong data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, long data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, double data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, bool data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static Message SendAndGetReply(this EasyTcpClient client, string data, TimeSpan? timeout = null,
            Encoding encoding = null, bool compression = false) =>
            client.SendAndGetReply((encoding ?? Encoding.UTF8).GetBytes(data), timeout, compression);

        /// <summary>
        /// Send data (IEasyTcpPacket) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        public static Message SendAndGetReply(this EasyTcpClient client, IEasyTcpPacket data, TimeSpan? timeout = null,
            bool compression = false) =>
            client.SendAndGetReply(data.Data, timeout, compression);
    }
}
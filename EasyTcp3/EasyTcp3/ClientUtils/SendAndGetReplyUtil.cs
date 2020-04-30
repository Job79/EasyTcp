using System;
using System.Text;
using System.Threading;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to send a message to a remote host and then return the reply
    /// </summary>
    public static class SendAndGetReplyUtil
    {
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public static Message SendAndGetReply(this EasyTcpClient client, byte[] data, TimeSpan? timeout = null)
        {
            if (client == null) throw new ArgumentException("Could not send: client is null");

            Message reply = null;
            using var signal = new ManualResetEventSlim();

            client.FireOnDataReceive = message =>
            {
                reply = message;
                client.FireOnDataReceive = client.FireOnDataReceiveEvent;
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Set();
            };
            client.Send(data);

            signal.Wait(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.FireOnDataReceive = client.FireOnDataReceiveEvent;
            return reply;
        }

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, ushort data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, short data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, uint data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, int data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, ulong data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, long data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, double data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendAndGetReply(this EasyTcpClient client, bool data, TimeSpan? timeout = null) =>
            client.SendAndGetReply(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        public static Message SendAndGetReply(this EasyTcpClient client, string data, TimeSpan? timeout = null,
            Encoding encoding = null) =>
            client.SendAndGetReply((encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}
using System;
using System.Text;
using System.Threading;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions to send a action to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendActionAndGetReplyUtil
    {
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, byte[] data,
            TimeSpan? timeout = null)
        {
            if (client == null) throw new ArgumentException("Could not send: client is null");

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
            client.SendAction(action, data);

            signal.Wait(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.ResetDataReceiveHandler();
            return reply;
        }

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, byte[] data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), data, timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, ushort data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, ushort data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, short data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, short data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, uint data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, uint data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, int data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, int data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, ulong data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, ulong data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, long data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, long data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, double data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, double data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, bool data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, bool data,
            TimeSpan? timeout = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, int action, string data,
            TimeSpan? timeout = null,
            Encoding encoding = null) =>
            client.SendActionAndGetReply(action, (encoding ?? Encoding.UTF8).GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then return the reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        public static Message SendActionAndGetReply(this EasyTcpClient client, string action, string data,
            TimeSpan? timeout = null,
            Encoding encoding = null) =>
            client.SendActionAndGetReply(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}
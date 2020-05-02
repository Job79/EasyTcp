using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTcp3.Actions.ActionUtils.Async
{
    /// <summary>
    /// Functions to async send a action to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendActionAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Default timeout used when no parameter is passed
        /// </summary>
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send action with data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns> 
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, byte[] data,
            TimeSpan? timeout = null)
        {
            if (client == null) throw new ArgumentException("Could not send: client is null");

            Message reply = null;
            using var signal = new SemaphoreSlim(0, 1); //Use SemaphoreSlim as async ManualResetEventSlim

            client.DataReceiveHandler = message =>
            {
                reply = message;
                client.ResetDataReceiveHandler();
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Release();
            };
            client.SendAction(action, data);

            await signal.WaitAsync(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.ResetDataReceiveHandler();
            return reply;
        }

        /// <summary>
        /// Send action with data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, ushort data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, short data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, uint data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, int data,
            TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, ulong data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, long data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, double data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendActionAndGetReplyAsync(this EasyTcpClient client, int action, bool data, TimeSpan? timeout = null) =>
            await client.SendActionAndGetReplyAsync(action, BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send action with data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendActionAndGetReplyAsync(this EasyTcpClient client, int action, string data,
            TimeSpan? timeout = null, Encoding encoding = null)
            => await client.SendActionAndGetReplyAsync(action, (encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}
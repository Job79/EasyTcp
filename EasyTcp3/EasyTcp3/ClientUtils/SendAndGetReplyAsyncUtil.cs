using System;
using System.Text;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils
{
    public static class SendAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Send data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, byte[] data, TimeSpan timeout)
            => await Task.Run(() => client.SendAndGetReplyAsync(data, timeout));

        /// <summary>
        /// Send data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ushort data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, short data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, uint data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, int data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ulong data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, long data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, double data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, bool data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, string data, TimeSpan timeout,
            Encoding encoding = null)
            => await client.SendAndGetReplyAsync((encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}
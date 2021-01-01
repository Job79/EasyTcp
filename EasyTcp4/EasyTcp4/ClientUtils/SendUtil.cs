using System.Text;
using EasyTcp4.PacketUtils;

namespace EasyTcp4.ClientUtils
{
    public static class SendUtil
    {
        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        public static void Send(this EasyTcpClient client, params byte[][] data) =>
            client.Protocol.SendMessage(client, data);

        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void Send(this EasyTcpClient client, byte[] data, bool compression = false)
        {
            if (compression) data = CompressionUtil.Compress(data);
            client.Protocol.SendMessage(client, data);
        }

        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void Send(this EasyTcpClient client, string data, Encoding encoding = null, bool compression = false)
            => client.Send((encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void Send(this EasyTcpClient client, IEasyPacket data, bool compression = false)
            => client.Send(data.Data, compression);

        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void Send(this EasyTcpClient client, object data, bool compression = false)
            => client.Send(client?.Serialize(data), compression);
    }
}

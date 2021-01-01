using System;
using System.Text;
using EasyTcp4.ClientUtils;
using EasyTcp4.PacketUtils;

namespace EasyTcp4.Actions.Utils
{
    public static class SendActionUtil
    {
        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, byte[] data = null, bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            client.Send(BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, byte[] data = null, bool compression = false) =>
            client.SendAction(action.ToActionCode(), data, compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, string data, Encoding encoding = null, bool compression = false)
            => client.SendAction(action, (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (default: UTF8)</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, string data, Encoding encoding = null, bool compression = false)
            => client.SendAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, IEasyPacket data, bool compression = false)
            => client.SendAction(action, data.Data, compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, IEasyPacket data, bool compression = false)
            => client.SendAction(action.ToActionCode(), data.Data, compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, object data, bool compression = false)
            => client.SendAction(action, client?.Serialize(data), compression);

        /// <summary>
        /// Send action to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, object data, bool compression = false)
            => client.SendAction(action.ToActionCode(), client?.Serialize(data), compression);
    }
}

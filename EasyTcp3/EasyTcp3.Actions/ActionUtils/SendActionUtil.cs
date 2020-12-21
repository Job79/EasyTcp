using System;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.PacketUtils;

namespace EasyTcp3.Actions.ActionUtils
{
    public static class SendActionUtil
    {
        /// <summary>
        /// Send action (byte[]) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, byte[] data = null,
            bool compression = false)
        {
            if (compression && data != null) data = CompressionUtil.Compress(data);
            client.Send(BitConverter.GetBytes(action), data);
        }

        /// <summary>
        /// Send action (byte[]) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, byte[] data = null,
            bool compression = false) =>
            client.SendAction(action.ToActionCode(), data, compression);

        /// <summary>
        /// Send action (string) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, string data, Encoding encoding = null,
            bool compression = false)
            => client.SendAction(action, (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action (string) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, string data, Encoding encoding = null,
            bool compression = false)
            => client.SendAction(action.ToActionCode(), (encoding ?? Encoding.UTF8).GetBytes(data), compression);

        /// <summary>
        /// Send action (IEasyTcpPacket) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, IEasyTcpPacket data, bool compression = false)
            => client.SendAction(action, data.Data, compression);

        /// <summary>
        /// Send action (IEasyTcpPacket) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, IEasyTcpPacket data, bool compression = false)
            => client.SendAction(action.ToActionCode(), data.Data, compression);
        
        /// <summary>
        /// Send action (object) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, int action, object data, bool compression = false)
            => client.SendAction(action, client?.Serialize(data), compression);

        /// <summary>
        /// Send action (object) to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action code as string</param>
        /// <param name="data">data to send to remote host</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        public static void SendAction(this EasyTcpClient client, string action, object data, bool compression = false)
            => client.SendAction(action.ToActionCode(), client?.Serialize(data), compression);
    }
}

#if !NETSTANDARD2_1
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions to send actions with serialized information
    /// </summary>
    public static class SerializationActionUtil
    {
        /// <summary>
        /// Send data (custom class) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="action">action id</param>
        /// <param name="o">custom class</param>
        public static void SendAction(this EasyTcpClient client, int action, object o)
            => SendActionUtil.SendAction(client, action, SerializationUtil.Serialize(o));

        /// <summary>
        /// Send data (custom class) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="action">action id</param>
        /// <param name="o">custom class</param>
        public static void SendAll(this EasyTcpServer server, int action, object o)
            => SendAllActionUtil.SendAllAction(server, action, SerializationUtil.Serialize(o));
    }
}
#endif
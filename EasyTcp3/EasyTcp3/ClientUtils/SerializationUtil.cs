#if NETCOREAPP3_1
using System.Text.Json;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.ClientUtils
{
    public static class SerializationUtil
    {
        public static void Send(this EasyTcpClient client, object o)
            => SendUtil.Send(client,JsonSerializer.SerializeToUtf8Bytes(o));

        public static void SendAll(this EasyTcpServer server, object o)
            => SendAllUtil.SendAll(server,JsonSerializer.SerializeToUtf8Bytes(o));
        
        public static T GetObject<T>(this Message m)
            => JsonSerializer.Deserialize<T>(m.Data);
    }
}
#endif
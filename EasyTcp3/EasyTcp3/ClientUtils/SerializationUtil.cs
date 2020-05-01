#if NETCOREAPP3_1
using System.Text.Json;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// This class contains support for transferring serialized objects with EasyTcp
    /// It uses System.Text.Json and is only available if compiled with a newer version of dotnet
    /// If you are using the nuget package and this functionality is excluded import this class into your project
    /// or rebuilt the package
    /// 
    /// If using an older version of dotnet, rebuild this class for NewtonSoft (It shouldn't be hard,
    /// only change the functions Serialize and Deserialize)
    /// If you ported this class and want to support the community feel free to open an issue with your solution
    /// ~ Job79
    /// </summary>
    public static class SerializationUtil
    {
        /// <summary>
        /// Serialize custom class
        /// </summary>
        /// <param name="data">custom class</param>
        /// <returns>custom class as byte[]</returns>
        private static byte[] Serialize(object data)
            => JsonSerializer.SerializeToUtf8Bytes(data);
        
        /// <summary>
        /// Deserialize byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T">custom class type</typeparam>
        /// <returns>byte[] as custom class</returns>
        private static T Deserialize<T>(byte[] data)
            => JsonSerializer.Deserialize<T>(data);
        
        
        /// <summary>
        /// Send data (custom class) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="o">custom class</param>
        public static void Send(this EasyTcpClient client, object o)
            => SendUtil.Send(client,Serialize(o));

        /// <summary>
        /// Send data (custom class) to all connected clients
        /// </summary>
        /// <param name="server"></param>
        /// <param name="o">custom class</param>
        public static void SendAll(this EasyTcpServer server, object o)
            => SendAllUtil.SendAll(server, Serialize(o));
        
        /// <summary>
        /// Received data as custom class
        /// </summary>
        /// <param name="m"></param>
        /// <typeparam name="T">custom class type</typeparam>
        /// <returns>received data as custom class</returns>
        public static T GetObject<T>(this Message m)
            => Deserialize<T>(m.Data);
    }
}
#endif
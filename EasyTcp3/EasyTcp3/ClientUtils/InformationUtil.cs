using System.Net;

namespace EasyTcp3.ClientUtils
{
    public static class InformationUtil
    {
        /// <summary>
        /// Get the ip of this client
        /// </summary>
        /// <param name="client"></param>s
        /// <returns>Ip of client</returns>
        public static IPAddress GetIp(this EasyTcpClient client) =>
            ((IPEndPoint) client.BaseSocket.RemoteEndPoint).Address;
    }
}
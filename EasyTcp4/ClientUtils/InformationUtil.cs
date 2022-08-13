using System.Net;
using System.Net.Sockets;

namespace EasyTcp4.ClientUtils
{
    public static class InformationUtil
    {
        /// <summary>
        /// Determines whether client is still connected to the remote endpoint
        /// </summary>
        /// <param name="client"></param>
        /// <returns>determines whether the client is still connected</returns>
        public static bool IsConnected(this EasyTcpClient client)
        {
            if (client?.BaseSocket == null) return false;
            if (!client.BaseSocket.Connected)
            {
                client.FireOnDisconnect();
                client.Dispose();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get endpoint of client
        /// </summary>
        /// <param name="client"></param>s
        /// <returns>endpoint of client</returns>
        public static IPEndPoint GetEndPoint(this EasyTcpClient client) => (IPEndPoint)client?.BaseSocket?.RemoteEndPoint;

        /// <summary>
        /// Get ip of client
        /// </summary>
        /// <param name="client"></param>
        /// <returns>ip of client</returns>
        public static IPAddress GetIp(this EasyTcpClient client) =>
            client?.GetEndPoint()?.Address;
    }
}

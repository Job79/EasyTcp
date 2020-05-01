using System.Net;
using System.Net.Sockets;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to receive information from an EasyTcpClient
    /// </summary>
    public static class InformationUtil
    {
        /// <summary>
        /// Determines if a client is still connected to an endpoint
        /// </summary>
        /// <param name="client"></param>
        /// <param name="poll">uses poll if set to true, can be more accurate but decreases performance</param>
        /// <returns>determines whether the client is still connected</returns>
        public static bool IsConnected(this EasyTcpClient client, bool poll = false)
        {
            if (client?.BaseSocket == null) return false;
            if (!client.BaseSocket.Connected || !poll && client.BaseSocket.Poll(0, SelectMode.SelectRead) &&
                client.BaseSocket.Available.Equals(0))
            {
                client.HandleDisconnect();
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Get the ip of a client
        /// </summary>
        /// <param name="client"></param>s
        /// <returns>ip of client</returns>
        public static IPAddress GetIp(this EasyTcpClient client) =>
            ((IPEndPoint) client.BaseSocket.RemoteEndPoint).Address;
    }
}
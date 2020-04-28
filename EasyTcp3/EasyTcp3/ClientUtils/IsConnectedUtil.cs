using System.Net.Sockets;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.ClientUtils
{
    public static class IsConnectedUtil
    {
        /// <summary>
        /// Determines if a client is still connected to an endpoint
        /// </summary>
        /// <param name="client"></param>
        /// <param name="poll">Uses poll if set to true, can be more accurate but decreases performance</param>
        /// <returns>true =  client is still connected, false = client is disconnected</returns>
        public static bool IsConnected(this EasyTcpClient client, bool poll = false)
        {
            if (client?.BaseSocket == null) return false;
            if (!client.BaseSocket.Connected || !poll && client.BaseSocket.Poll(0, SelectMode.SelectRead) &&
                client.BaseSocket.Available.Equals(0))
            {
                OnReceiveUtil.HandleDisconnect(client);
                return false;
            }
            return true;
        }
    }
}
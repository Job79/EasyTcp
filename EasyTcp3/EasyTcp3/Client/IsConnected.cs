using System.Net.Sockets;

namespace EasyTcp3.Client
{
    public static class _IsConnected
    {
        /// <summary>
        /// Determines if a client is still connected to an endpoint
        /// </summary>
        /// <param name="client"></param>
        /// <param name="poll">Uses poll if set to true, can be more accurate but decreases performance</param>
        /// <returns>true =  client is still connected, false = client is disconnected</returns>
        ///
        /// <example>
        /// using var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///    
        /// Assert.IsTrue(client.IsConnected(true));
        /// client.Dispose(); //Disconnect
        /// Assert.IsFalse(client.IsConnected());
        /// </example>
        public static bool IsConnected(this EasyTcpClient client, bool poll = false)
        {
            if (client?.BaseSocket == null) return false;
            if (!client.BaseSocket.Connected || !poll && client.BaseSocket.Poll(0, SelectMode.SelectRead) &&
                client.BaseSocket.Available.Equals(0))
            {
                Internal._OnReceive.HandleDisconnect(client);
                return false;
            }
            else return true;
        }
    }
}
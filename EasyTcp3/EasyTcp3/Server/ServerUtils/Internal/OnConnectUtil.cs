using System;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.Server.ServerUtils.Internal
{
    public static class OnConnectUtil
    {
        /// <summary>
        /// Triggered when a new client connects
        /// </summary>
        /// <param name="ar"></param>
        internal static void OnClientConnect(IAsyncResult ar)
        {
            var server = ar.AsyncState as EasyTcpServer;
            if (server == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(server.BaseSocket.EndAccept(ar));
                
                //TODO: Cancel connect in event handler
                server.FireOnConnect(client);
                server.ConnectedClients.Add(client);
                OnReceiveUtil.StartListening(client);
            }
            catch (Exception ex)
            {
                if (server.IsRunning) server.FireOnError(ex);
            }

            server.BaseSocket.BeginAccept(OnClientConnect, server); //Accept next client
        }
    }
}
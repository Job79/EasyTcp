using System;
using EasyTcp3.ClientUtils;
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
                client.OnDataReceive += (sender, message) => server.FireOnDataReceive(message);//TODO: Create and edit tests
                client.OnDisconnect += (sender, c) => server.FireOnDisconnect(c);
                client.OnError += (sender, exception) => server.FireOnError(exception);
                
                server.FireOnConnect(client);
                if (client.IsConnected()) //Check if user aborted
                {
                    server.ConnectedClients.Add(client);
                    OnReceiveUtil.StartListening(client);
                }
            }
            catch (Exception ex)
            {
                if (server.IsRunning) server.FireOnError(ex);
            }

            server.BaseSocket.BeginAccept(OnClientConnect, server); //Accept next client
        }
    }
}
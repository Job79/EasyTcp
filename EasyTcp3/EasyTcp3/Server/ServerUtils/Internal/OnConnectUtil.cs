using System;
using EasyTcp3.Protocols;

namespace EasyTcp3.Server.ServerUtils.Internal
{
    /// <summary>
    /// Internal functions to accept new connections
    /// </summary>
    internal static class OnConnectUtil
    {
        /// <summary>
        /// Function that gets triggered when data a new client connects
        /// </summary>
        /// <param name="ar"></param>
        internal static void OnClientConnect(IAsyncResult ar)
        {
            var server = ar.AsyncState as EasyTcpServer;
            if (server?.BaseSocket == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(server.BaseSocket.EndAccept(ar), (IEasyTcpProtocol) server.Protocol.Clone());
                client.OnDataReceive += (_, message) => server.FireOnDataReceive(message);
                client.OnDisconnect += (_, c) => server.FireOnDisconnect(c);
                client.OnError += (_, exception) => server.FireOnError(exception);

                client.Protocol.OnConnectServer(client);
                server.FireOnConnect(client);
                if (client.BaseSocket != null) //Check if user aborted OnConnect with Client.Dispose()
                {
                    lock (server.ConnectedClients) server.ConnectedClients.Add(client);
                }
            }
            catch (Exception ex)
            {
                server.FireOnError(ex);
            }

            server.BaseSocket.BeginAccept(OnClientConnect, server); //Accept next client
        }
    }
}
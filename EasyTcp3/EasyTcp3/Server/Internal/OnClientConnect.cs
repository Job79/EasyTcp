using System;

namespace EasyTcp3.Server.Internal
{
    internal static class OnClientConnect
    {
        /// <summary>
        /// Triggered when a new client connects
        /// </summary>
        /// <param name="ar"></param>
        internal static void _OnClientConnect(IAsyncResult ar)
        {
            var server = ar.AsyncState as EasyTcpServer;
            if (server == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(server.BaseSocket.EndAccept(ar)) {Buffer = new byte[2]};

                Client.Internal._OnReceive.StartListening(client);
                server.ConnectedClients.Add(client);
                server.FireOnConnect(client);
            }
            catch (Exception ex)
            {
                if (server.IsRunning) server.FireOnError(ex);
            }

            server.BaseSocket.BeginAccept(_OnClientConnect, server); //Accept next client
        }
    }
}
namespace EasyTcp3.Server
{
    public static class _Stop
    {
        /// <summary>
        /// Stop server
        /// </summary>
        /// <param name="server"></param>
        public static void Stop(this EasyTcpServer server)
        {
            if(server.BaseSocket == null || !server.IsRunning) return;
            server.IsRunning = false;
            foreach (var client in server.ConnectedClients) client.Dispose();
            server.ConnectedClients.Clear();
        }
    }
};
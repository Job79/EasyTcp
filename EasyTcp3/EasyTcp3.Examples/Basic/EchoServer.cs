using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Basic
{
    /// <summary>
    /// This class contains a basic echo server,
    /// everything it receives is send back to the client
    /// </summary>
    public static class EchoServer
    {
        private const ushort Port = 5_001;

        public static void StartEchoServer()
        {
            var server = new EasyTcpServer().Start(Port); //Start server on port 5001

            // Send received data back
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
        }
    }
}
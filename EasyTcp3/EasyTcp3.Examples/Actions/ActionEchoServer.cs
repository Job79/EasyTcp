using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    public class ActionEchoServer // TODO
    {
        private const ushort Port = 6_001;

        public static void StartEchoServer()
        {
            var server = new EasyTcpActionServer();
            server.Start(Port);//Start server on port 6001
        }

        [EasyTcpAction("ECHO")]
        public static void Echo(object sender, Message e)
        {
            e.Client.Send(e.Data);
        }
        
        [EasyTcpAction("BROADCAST")]
        public static void Broadcast(object sender, Message e)
        {
            var server = sender as EasyTcpServer;
            server.SendAll(e.Data);
        }
    }
}
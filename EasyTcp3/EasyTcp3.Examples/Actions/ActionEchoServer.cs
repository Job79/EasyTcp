using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    public class ActionEchoServer // TODO
    {
        private const ushort Port = 5_001;

        public static void StartEchoServer()
        {
            var server = new EasyTcpActionServer();
            server.Start(Port);//Start server on port 5001
        }

        [EasyTcpAction(0)]
        public static void Echo(object sender, Message e)
        {
            e.Client.Send(e.Data);
        }
    }
}
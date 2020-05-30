using System;
using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Readme
{
    public class EasyTcpActions
    {
        const ushort Port = 100;

        public void Connect()
        {
            using var client = new EasyTcpClient();
            if(!client.Connect(IPAddress.Loopback, Port)) return; 
            client.SendAction("echo","Hello server");
            client.SendAction("broadcast","Hello everyone"); 
        }
        
        public void Run()
        {
            using var server = new EasyTcpActionServer().Start(Port);
            server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
            server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
        }

        [EasyTcpAction("echo")]
        public void EchoAction(Message message)
        {
            message.Client.Send(message);
        }

        [EasyTcpAction("broadcast")]
        public void BroadCastAction(object sender, Message message)
        {
            var server = (EasyTcpServer) sender;
            server.SendAll(message);
        }
    }
}
using System;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test
{
    public class CancelDisconnect
    {
        [Test]
        public void TestDisconnect()
        {
            EasyTcpServer server = new EasyTcpServer();
            server.Start(1234);
            server.OnConnect += (sender, client) => client.Dispose();
            server.OnDisconnect += (sender, client) => Console.WriteLine("Disconnect");
            
            EasyTcpClient client = new EasyTcpClient();
            client.Connect(IPAddress.Any, 1234);
            
            TestHelper.WaitWhileTrue(()=>client.IsConnected());
            Assert.IsFalse(client.IsConnected());
        }
    }
}
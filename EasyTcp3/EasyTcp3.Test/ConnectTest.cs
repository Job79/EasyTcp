using System.Collections.Generic;
using System.Net;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test
{
    public class ConnectTest
    {
        [Test]
        public void TestConnect()
        {
            var port = TestHelper.GetPort();
            var client = new EasyTcpClient();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);
            
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout),"Client did not connect");
        }

        [Test]
        public void TestEventHandlerClient()
        {
            var port = TestHelper.GetPort();
            using var client = new EasyTcpClient();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            byte x = 0;
            client.OnConnect += (o, c) => x++;
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout), "Client did not connect");
            
            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(0,x, "OnConnect event was not fired");
            Assert.AreEqual(1,x);
        }
        
        [Test]
        public void TestEventHandlerServer()
        {
            var port = TestHelper.GetPort();
            using var client = new EasyTcpClient();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            byte x = 0;
            server.OnConnect += (o, c) => x++;
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout),"Client did not connect");
            
            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(0,x, "OnConnect event was not fired");
            Assert.AreEqual(1,x);
        }

        [Test]
        public void TestMultipleConnections()
        {
            const int amountOfClients = 5000;
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            var clients = new List<EasyTcpClient>();
            for (int i = 0; i < amountOfClients; i++)
            {
                var client = new EasyTcpClient();
                Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout),"Client did not connect");
                clients.Add(client);
            }

            Assert.AreEqual(server.ConnectedClientsCount, amountOfClients,
                "Number of connectedClients is not the same as the amount of clients connected to the server");
            foreach (var client in clients) client.Dispose();
        }
    }
}
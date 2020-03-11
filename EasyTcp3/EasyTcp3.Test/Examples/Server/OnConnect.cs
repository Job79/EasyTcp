using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples.Server
{
    public class OnConnect
    {
        [Test]
        public void OnConnect1()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            int connectCount = 0;
            server.OnConnect += (sender, client) =>
                {
                    Interlocked.Increment(ref connectCount);//Async lambda, thread safe increase integer
                    Console.WriteLine($"Client {connectCount} connected");
                };

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            
            TestHelper.WaitWhileTrue(()=>connectCount == 0);
            Assert.AreEqual(1,connectCount);
        }
        
        /*[Test]
        public void OnConnectMultipleConnections()
        {
            const int connectionsCount = 1_000;
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port, false, 1000);

            int connectCount = 0;
            server.OnConnect += (sender, client)
                => Interlocked.Increment(ref connectCount);

            var clients = new List<EasyTcpClient>();
            for (int i = 0; i < connectionsCount; i++)
            {
                var client = new EasyTcpClient();
                Assert.IsTrue(client.Connect(IPAddress.Any, port));
                clients.Add(client);
            }

            TestHelper.WaitWhileFalse(()=>connectCount == connectionsCount);
            Assert.AreEqual(connectionsCount,connectCount);
            
            foreach (var client in clients) client.Dispose();
        }*/
    }
}
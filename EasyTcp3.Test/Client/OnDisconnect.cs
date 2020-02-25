using System;
using System.Net;
using System.Threading;
using EasyTcp.Server;
using EasyTcp3.Client;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
    public class OnDisconnect
    {
        [Test]
        public void DisposeTest()
        {
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, 1521,10);
            
            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, 1521);

            var x = false;
            client.OnDisconnect += (sender, client) => x = true;
            client.Dispose();
            
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);

            Assert.IsTrue(x);
        }
        
        [Test]
        public void ServerDisconnect()
        {
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, 1521,10);
            server.OnError += (_,e)=>Console.Write(e);
            
            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, 1521);

            var x = false;
            client.OnDisconnect += (sender, client) => x = true;
            server.Listener.Close();
            
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsTrue(x);
        }
    }
}
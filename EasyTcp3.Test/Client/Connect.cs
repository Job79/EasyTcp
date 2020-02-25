using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using EasyTcp3.Client;
using EasyTcp.Server;

namespace EasyTcp3.Test.Client
{
    public class Connect
    {
        [Test]
        public void ConnectTest_Valid()
        {
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, 2131,100);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, 2131),"Client did not connect to server");
        }
        
        [Test]
        public void ConnectTest_Invalid()
        {
            using var client = new EasyTcpClient();
            Assert.IsFalse(client.Connect(IPAddress.Any, 2132), "Client connected(returned true) to an invalid server");
        }
        
        [Test]
        public void ConnectEventHandlerTest_Valid()
        {
            var server = new EasyTcpServer();
            server.Start("0.0.0.0",2133,100);
            
            using var client = new EasyTcpClient();
            var x = false;
            client.OnConnect += (sender, e) => x = true;

            client.Connect(IPAddress.Any, 2133);
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsTrue(x,"Connect event was not fired");
        }
        
        [Test]
        public void ConnectEventHandlerTest_InValid()
        {
            using var client = new EasyTcpClient();
            var x = false;
            client.OnConnect += (sender, e) => x = true;

            client.Connect(IPAddress.Any, 2134);
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsFalse(x,"Connect event was fired while connect failed");
        }
    }
}
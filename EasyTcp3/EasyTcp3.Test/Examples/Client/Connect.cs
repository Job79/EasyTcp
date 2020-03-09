using System;
using System.Net;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples
{
    public class Connect
    {
        private ushort port;
        [SetUp]
        public void Setup()
        {
            port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);
        }
        
        [Test]
        public void Connect1()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect(IPAddress.Any, port, TimeSpan.FromSeconds(1));
            Assert.IsTrue(isConnected);
        }
        
        [Test]
        public void Connect2()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect(IPAddress.Any, port); //Use default timeout of 5 seconds
            Assert.IsTrue(isConnected);
        }
        
        [Test]
        public void Connect3()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect("127.0.0.1", port, TimeSpan.FromSeconds(1));
            Assert.IsTrue(isConnected);
        }
        
        [Test]
        public void Connect4()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect("127.0.0.1", port); //Use default timeout of 5 seconds
            Assert.IsTrue(isConnected);
        }
    }
}
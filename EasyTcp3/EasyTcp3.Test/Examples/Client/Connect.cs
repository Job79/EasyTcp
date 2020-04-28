using System;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples.Client
{
    public class Connect
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(_port);
        }

        [Test]
        public void Connect1()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect(IPAddress.Any, _port, TimeSpan.FromSeconds(1));
            Assert.IsTrue(isConnected);
        }

        [Test]
        public void Connect2()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect(IPAddress.Any, _port); //Use default timeout of 5 seconds
            Assert.IsTrue(isConnected);
        }

        [Test]
        public void Connect3()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect("127.0.0.1", _port, TimeSpan.FromSeconds(1));
            Assert.IsTrue(isConnected);
        }

        [Test]
        public void Connect4()
        {
            using var client = new EasyTcpClient();
            bool isConnected = client.Connect("127.0.0.1", _port); //Use default timeout of 5 seconds
            Assert.IsTrue(isConnected);
        }
    }
}
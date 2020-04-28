using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples.Client
{
    public class IsConnected
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
        public void IsConnected1()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, _port);
            
            Assert.IsTrue(client.IsConnected(true));
            client.Dispose(); //Disconnect
            Assert.IsFalse(client.IsConnected());
        }
    }
}
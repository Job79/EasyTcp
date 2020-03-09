using System.Net;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples
{
    public class IsConnected
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
        public void IsConnected1()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);
            
            Assert.IsTrue(client.IsConnected(true));
            client.Dispose(); //Disconnect
            Assert.IsFalse(client.IsConnected());
        }
    }
}
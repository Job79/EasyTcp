using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for the Information functions
    /// </summary>
    public class Information
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer().Start(_port);
        }

        [Test]
        public void IsConnected1()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, _port);

            Assert.IsTrue(client.IsConnected(true));
            client.Dispose(); // Disconnect
            Assert.IsFalse(client.IsConnected());
        }

        [Test]
        public void TestIp()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            Assert.AreEqual(IPAddress.Loopback, client.GetIp());
            Assert.AreEqual(IPAddress.Loopback, server.GetConnectedClients().First().GetIp());
        }
    }
}
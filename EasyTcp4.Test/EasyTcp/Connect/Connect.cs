using System.Net;
using EasyTcp4.ClientUtils;
using EasyTcp4.ServerUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Connect
{
    public class Connect
    {
        [Test]
        public void ConnectEndpoint()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(client.Connect(new IPEndPoint(IPAddress.Loopback, port)));
            Assert.IsTrue(client.IsConnected());
        }

        [Test]
        public void ConnectIPAddress()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            Assert.IsTrue(client.IsConnected());
        }

        [Test]
        public void ConnectIPAddressString()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(client.Connect("127.0.0.1", port));
            Assert.IsTrue(client.IsConnected());
        }
    }
}

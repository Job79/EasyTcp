using System.Net;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.ClientUtils.Async;
using EasyTcp4.ServerUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Connect
{
    public class ConnectAsync
    {
        [Test]
        public async Task ConnectAsyncEndpoint()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, port)));
            Assert.IsTrue(client.IsConnected());
        }

        [Test]
        public async Task ConnectIPAddress()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(await client.ConnectAsync(IPAddress.Loopback, port));
            Assert.IsTrue(client.IsConnected());
        }

        [Test]
        public async Task ConnectIPAddressString()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            Assert.IsTrue(await client.ConnectAsync("127.0.0.1", port));
            Assert.IsTrue(client.IsConnected());
        }
    }
}

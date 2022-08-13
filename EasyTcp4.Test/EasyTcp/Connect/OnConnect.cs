using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils.Async;
using EasyTcp4.ServerUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Connect
{
    public class OnConnect
    {
        [Test]
        public async Task OnConnectClient()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            int raised = 0;
            client.OnConnect += (_, c) => Interlocked.Increment(ref raised);
            await client.ConnectAsync(IPAddress.Loopback, port);

            Assert.AreEqual(1, raised);
        }

        [Test]
        public async Task OnConnectServer()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();

            int raised = 0;
            server.OnConnect += (_, c) => Interlocked.Increment(ref raised);
            await client.ConnectAsync(IPAddress.Loopback, port);

            await TestHelper.WaitWhileFalse(() => raised == 1);
            Assert.AreEqual(1, raised);
        }

        [Test]
        public async Task OnConnectServerMultipleConnections()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            int raised = 0;
            server.OnConnect += (_, c) => Interlocked.Increment(ref raised);
            for (int i = 0; i < 5; i++)
            {
                using var client = new EasyTcpClient();
                await client.ConnectAsync(IPAddress.Loopback, port);
            }

            await TestHelper.WaitWhileFalse(() => raised == 5);
            Assert.AreEqual(raised, 5);
        }
    }
}

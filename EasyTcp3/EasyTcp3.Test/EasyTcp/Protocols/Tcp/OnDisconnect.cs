using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Protocols.Tcp
{
    /// <summary>
    /// Tests for the OnDisconnect event
    /// </summary>
    public class OnDisconnect
    {
        [Test]
        public void OnDisconnectServer()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            int x = 0;
            server.OnDisconnect += (o, c) => Interlocked.Increment(ref x);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
        
        [Test]
        public void OnDisconnectClient()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            var client = new EasyTcpClient();
            int x = 0;
            client.OnDisconnect += (o, c) => Interlocked.Increment(ref x);

            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            TestHelper.WaitWhileFalse(() => server.ConnectedClientsCount == 1);
            server.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
    }
}
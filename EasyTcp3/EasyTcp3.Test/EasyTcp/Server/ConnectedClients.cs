using System.Net;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Server
{
    /// <summary>
    /// Tests for the server ConnectedClients list
    /// </summary>
    public class ConnectedClients
    {
        [Test]
        public void TestConnectedClients()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            Assert.IsEmpty(server.GetConnectedClients());

            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Loopback, port);
            TestHelper.WaitWhileFalse(() => server.ConnectedClientsCount == 1);
            Assert.AreEqual(1, server.ConnectedClientsCount);

            using var client2 = new EasyTcpClient();
            client2.Connect(IPAddress.Loopback, port);
            TestHelper.WaitWhileFalse(() => server.ConnectedClientsCount == 2);
            Assert.AreEqual(2, server.ConnectedClientsCount);

            client.Dispose();
            TestHelper.WaitWhileTrue(() => server.ConnectedClientsCount == 2);
            Assert.AreEqual(1, server.ConnectedClientsCount);

            client2.Dispose();
            TestHelper.WaitWhileTrue(() => server.ConnectedClientsCount == 1);
            Assert.AreEqual(0, server.ConnectedClientsCount);
        }
    }
}
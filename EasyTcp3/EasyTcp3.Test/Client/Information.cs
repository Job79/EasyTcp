using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
    public class Information
    {
        [Test]
        public void TestIp()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            
            Assert.AreEqual(IPAddress.Loopback, client.GetIp());
            Assert.AreEqual(IPAddress.Loopback, server.GetConnectedClients().First().GetIp());
        }
    }
}
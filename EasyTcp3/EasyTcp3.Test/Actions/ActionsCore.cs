using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests for ActionsCore.cs
    /// </summary>
    public class ActionsCore
    {
        [Test]
        public void TestServer()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer();
            server.Start(port);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            string data = "test";
            var reply = client.SendActionAndGetReply(0, data);
            Assert.AreEqual(data, reply.ToString());
        }
        
        [Test]
        public void TestClient()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            using var client = new EasyTcpActionClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            string data = "test";
            foreach (var c in server.GetConnectedClients())
            {
                var reply = c.SendActionAndGetReply(0, data);
                Assert.AreEqual(data, reply.ToString());
            }
        }
    }
}
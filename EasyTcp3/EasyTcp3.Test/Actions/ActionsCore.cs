using System;
using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests that determines whether actions are executed on message receive 
    /// </summary>
    public class ActionsCore
    {
        [Test]
        public void TestServerActionsTrigger()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            string data = "test";
            var reply = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, reply.ToString());
        }

        [Test]
        public void TestClientActionsTrigger()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            using var client = new EasyTcpActionClient();
            client.OnDataReceive += (sender, message) => Console.WriteLine("123");
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            string data = "test";
            foreach (var c in server.GetConnectedClients())
            {
                var reply = c.SendActionAndGetReply("ECHO", data);
                Assert.AreEqual(data, reply.ToString());
            }
        }
    }
}
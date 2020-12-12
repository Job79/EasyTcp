using System;
using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions
{
    public class ActionsTrigger
    {
        [Test]
        public void TriggerActions_Server()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port);
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var data = "test";
            var reply = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, reply.ToString());
        }

        [Test]
        public void TriggerActions_Client()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpActionClient();
            client.OnDataReceive += (sender, message) => Console.WriteLine("123");
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var data = "test";
            foreach (var c in server.GetConnectedClients())
            {
                var reply = c.SendActionAndGetReply("ECHO", data);
                Assert.AreEqual(data, reply.ToString());
            }
        }
    }
}

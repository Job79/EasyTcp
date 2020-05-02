using System;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.Actions.ActionUtils.Async;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    public class Interceptor
    {
        [Test]
        public async Task TestInterceptorFalse()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer
                {Interceptor = (actionCode, message) => false }; // Create useless server
            server.Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            var message = await client.SendActionAndGetReplyAsync("ECHO", "data", TimeSpan.FromMilliseconds(500));
            Assert.IsNull(message); // Client did not receive a message
        }

        [Test]
        public void TestInterceptorTrue()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer
                {Interceptor = (actionCode, message) => true};
            server.Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            var message = client.SendActionAndGetReply("ECHO", "data");
            Assert.IsNotNull(message); // Client did receive a message
        }
    }
}
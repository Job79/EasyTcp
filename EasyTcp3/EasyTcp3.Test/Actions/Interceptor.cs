using System;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.Actions.ActionUtils.Async;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests that determine whether the interceptor works correctly
    /// </summary>
    public class Interceptor
    {
        [Test]
        public async Task TestInterceptorFalse()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer
            {
                Interceptor = action => false
            }.Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            var message = await client.SendActionAndGetReplyAsync("ECHO", "data", TimeSpan.FromMilliseconds(500));
            Assert.IsNull(message);
        }

        [Test]
        public void TestInterceptorTrue()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer
            {
                Interceptor = action => action.ActionCode.IsEqualToAction("ECHO") && action.ToString() == "data"
            }.Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            var message = client.SendActionAndGetReply("ECHO", "data");
            Assert.IsNotNull(message);
        }
    }
}
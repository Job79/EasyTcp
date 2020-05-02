using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp
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
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            int x = 0;
            server.OnDisconnect += (o, c) => Interlocked.Increment(ref x);
            
            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
    }
}
using System.Net;
using System.Threading;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests that determine whether OnUnknownAction is working correctly
    /// </summary>
    public class OnUnknownAction
    {
        [Test]
        public void OnUnknownActionTrue()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer();
            server.Start(port);

            int triggeredCounter = 0;
            server.OnUnknownAction += (sender, c) =>
            {
                if(c.ActionCode.IsEqualToAction("INVALIDACTION") && c.ToString() == "data") Interlocked.Increment(ref triggeredCounter);
            };

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            client.SendAction("INVALIDACTION", "data");

            TestHelper.WaitWhileFalse(() => triggeredCounter == 1);
            Assert.AreEqual(1, triggeredCounter);
        }

        [Test]
        public void OnUnknownActionFalse()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer();
            server.Start(port);

            int triggeredCounter = 0;
            server.OnUnknownAction += (sender, c) => Interlocked.Increment(ref triggeredCounter);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            client.SendAction("ECHO", "data");

            TestHelper.WaitWhileTrue(() => triggeredCounter == 0);
            Assert.AreEqual(0, triggeredCounter);
        }
    }
}
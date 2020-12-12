using System.Net;
using System.Threading;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions
{
    public class OnUnknownAction
    {
        [Test]
        public void TriggerOnUnknownAction_InvalidValidAction()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port);
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));
            var triggeredCounter = 0;

            server.OnUnknownAction += (sender, c) =>
            {
                if(c.GetActionCode().IsEqualToAction("INVALIDACTION") && c.ToString() == "data")
                    Interlocked.Increment(ref triggeredCounter);
            };
            client.SendAction("INVALIDACTION", "data");

            TestHelper.WaitWhileFalse(() => triggeredCounter == 1);
            Assert.AreEqual(1, triggeredCounter);
        }

        [Test]
        public void TriggerOnUnknownAction_ValidAction()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port);
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));
            int triggeredCounter = 0;

            server.OnUnknownAction += (sender, c) => Interlocked.Increment(ref triggeredCounter);
            client.SendAction("ECHO", "data");

            TestHelper.WaitWhileTrue(() => triggeredCounter == 0);
            Assert.AreEqual(0, triggeredCounter);
        }
    }
}

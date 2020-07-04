using System.Threading;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions.Filter
{
    public class FilterTest
    {
        [Test]
        public void TestFilters()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port); 
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));
            client.SendAction("AUTH");
            TestHelper.WaitWhileTrue(() => _counter == 0);
            Assert.AreEqual(0, _counter);
            
            client.SendAction("LOGIN");
            client.SendAction("AUTH");
            client.SendAction("AUTH");
            TestHelper.WaitWhileFalse(() => _counter == 2);
            Assert.AreEqual(2, _counter);
        }

        private static int _counter;

        [EasyTcpAction("LOGIN")]
        public void Login(Message message) => message.Client.Session["role"] = "user";

        [EasyTcpTestAuthorization]
        [EasyTcpAction("AUTH")]
        public void Auth() => Interlocked.Increment(ref _counter);
    }

    public class EasyTcpTestAuthorization : EasyTcpActionFilter
    {
        public override bool HasAccess(object sender, ActionMessage message)
        {
            if (message.Client.Session.TryGetValue("role", out object role) && role as string == "user") return true;
            else return false;
        }
    }
}
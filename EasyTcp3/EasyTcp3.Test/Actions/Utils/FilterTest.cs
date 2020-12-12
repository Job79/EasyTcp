using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions.Utils
{
    public class FilterTest
    {
        [Test]
        public void RunFilters_WithLogin()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port); 
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            client.SendAction("LOGIN");
            var reply = client.SendActionAndGetReply("AUTH"); // Trigger pass

            TestHelper.WaitWhileFalse(() => reply == null);
            Assert.AreEqual("auth", reply.ToString());
        }

        [Test]
        public void RunFilters_WithoutLogin()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpActionServer().Start(port); 
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var reply = client.SendActionAndGetReply("AUTH"); // Trigger pass

            TestHelper.WaitWhileFalse(() => reply == null);
            Assert.IsNull(reply);
        }

        private int _counter;

        [EasyTcpAction("LOGIN")]
        public void Login(Message message) => message.Client.Session["role"] = "user";

        [EasyTcpTestAuthorization]
        [EasyTcpAction("AUTH")]
        public void Auth(Message message) => message.Client.Send("auth");
    }

    public class EasyTcpTestAuthorization : EasyTcpActionFilter
    {
        public override bool HasAccess(object sender, Message message)
        {
            if (message.Client.Session.TryGetValue("role", out object role) && role as string == "user") return true;
            else return false;
        }
    }
}

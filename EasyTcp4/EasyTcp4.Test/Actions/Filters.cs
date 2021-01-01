using System.Threading.Tasks;
using EasyTcp4.Actions;
using EasyTcp4.Actions.Utils;
using EasyTcp4.Actions.Utils.Async;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.Actions
{
    public class Filters
    {
        [Test]
        public async Task FilterAllowRequest()
        {
            using var conn = await TestHelper.GetTestConnection(server: new EasyTcpActionServer());

            conn.Client.SendAction("LOGIN");
            var reply = await conn.Client.SendActionAndGetReplyAsync("AUTH");
            await TestHelper.WaitWhileTrue(() => reply == null);
            Assert.AreEqual("has access", reply.ToString());
        }

        [Test]
        public async Task FilterBlockRequest()
        {
            using var conn = await TestHelper.GetTestConnection(server: new EasyTcpActionServer());

            var reply = await conn.Client.SendActionAndGetReplyAsync("AUTH");
            await TestHelper.WaitWhileTrue(() => reply == null);
            Assert.AreEqual("no access", reply.ToString());
        }

        [EasyAction("LOGIN")]
        public void Login(Message message) => message.Client.Session["role"] = "user";

        [Authorization]
        [EasyAction("AUTH")]
        public void Auth(Message m) => m.Client.Send("has access");

        public class Authorization : EasyActionFilter
        {
            public override bool HasAccess(object s, Message m)
            {
                if (m.Client.Session.TryGetValue("role", out object role) && role as string == "user") return true;
                else
                {
                    m.Client.Send("no access");
                    return false;
                }
            }
        }
    }
}

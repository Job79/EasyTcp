using System.Threading.Tasks;
using EasyTcp4.Actions;
using EasyTcp4.Actions.Utils.Async;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.Actions
{
    public class EchoAction
    {
        [EasyAction("ECHO")]
        public void Echo(Message e) => e.Client.Send(e);

        [Test]
        public async Task TestEcho()
        {
            using var conn = await TestHelper.GetTestConnection(server: new EasyTcpActionServer());

            var reply = await conn.Client.SendActionAndGetReplyAsync("ECHO", "test echo data");
            Assert.AreEqual("test echo data", reply.ToString());
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.Actions;
using EasyTcp4.Actions.Utils;
using NUnit.Framework;

namespace EasyTcp4.Test.Actions.Events
{
    public class OnUnknownAction
    {
        [Test]
        public async Task TriggerOnUnknownAction()
        {
            using var conn = await TestHelper.GetTestConnection(server: new EasyTcpActionServer());

            int triggered = 0;
            ((EasyTcpActionServer)conn.Server).OnUnknownAction += (_, m)
                => Interlocked.Increment(ref triggered);
            conn.Client.SendAction("INVALIDACTION", "data");

            await TestHelper.WaitWhileFalse(() => triggered == 1);
            Assert.AreEqual(1, triggered);
        }

        [Test]
        public async Task NotTriggerOnUnknownAction()
        {
            using var conn = await TestHelper.GetTestConnection(server: new EasyTcpActionServer());

            int triggered = 0;
            ((EasyTcpActionServer)conn.Server).OnUnknownAction += (_, m)
                => Interlocked.Increment(ref triggered);
            conn.Client.SendActionAndGetReply("ECHO", "data");

            Assert.AreEqual(0, triggered);
        }
    }
}

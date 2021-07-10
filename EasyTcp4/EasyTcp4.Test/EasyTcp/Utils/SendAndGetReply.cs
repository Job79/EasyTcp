using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Utils
{
    public class SendAndGetReply
    {
        [Test]
        public async Task SendStringAndGetReply()
        {
            using var conn = await TestHelper.GetTestConnection();
            conn.Server.OnDataReceive += (_, m) => m.Client.Send(m);

            var message = "test message";
            var reply = conn.Client.SendAndGetReply(message);

            Assert.AreEqual(message, reply.ToString());
        }

        [Test]
        public async Task SendAndGetReplyDoesNotTriggerOnDataReceive()
        {
            using var conn = await TestHelper.GetTestConnection();
            conn.Server.OnDataReceive += (_, m) => m.Client.Send(m);
            
            int triggered = 0;
            conn.Client.OnDataReceive += (_, m) => Interlocked.Increment(ref triggered);
            var reply = conn.Client.SendAndGetReply("test message");

            Assert.AreEqual(0, triggered);
        }

        [Test]
        public async Task SendAndGetReplyInsideOnDataReceive()
        {
            using var conn = await TestHelper.GetTestConnection();
            conn.Server.OnDataReceive += (_, m) => m.Client.Send(m);
            
            var message = "test message";
            Message reply = null;
            conn.Client.OnDataReceive += (_, m) => 
            {
                conn.Client.Protocol.EnsureDataReceiverIsRunning(conn.Client);
                reply = conn.Client.SendAndGetReply(message);
            };
            conn.Client.Send("Trigger client onDataReceive");

            await TestHelper.WaitWhileTrue(()=>reply == null);
            Assert.AreEqual(message, reply.ToString());
        }
    }
}

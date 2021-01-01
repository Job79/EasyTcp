using System.Threading.Tasks;
using EasyTcp4.Actions;
using EasyTcp4.Actions.Utils;
using EasyTcp4.Actions.Utils.Async;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.Actions.Events
{
    public class Interceptor
    {
        [Test]
        public async Task InterceptorBlockRequest()
        {
            using var conn = await TestHelper.GetTestConnection(server:
                    new EasyTcpActionServer
                    {
                        Interceptor = message =>
                        {
                            message.Client.Send("no access");
                            return false;
                        }
                    });

            var message = await conn.Client.SendActionAndGetReplyAsync("ECHO", "has access");
            Assert.AreEqual("no access", message.ToString());
        }

        [Test]
        public async Task InterceptorAllowRequest()
        {
            using var conn = await TestHelper.GetTestConnection(server:
                    new EasyTcpActionServer
                    {
                        Interceptor = message => message.IsAction("ECHO") 
                    });

            var message = await conn.Client.SendActionAndGetReplyAsync("ECHO", "has access");
            Assert.AreEqual("has access", message.ToString());
        }
    }
}

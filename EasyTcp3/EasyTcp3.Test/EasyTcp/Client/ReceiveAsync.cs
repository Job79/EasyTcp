using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client
{
    public class ReceiveAsync
    {
        [Test]
        public async Task TestReceive()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            var receivedData = client.ReceiveAsync();
            TestHelper.WaitWhileFalse(() => server.ConnectedClientsCount == 1);
            server.SendAll("Data");
            Assert.AreEqual("Data", (await receivedData).ToString());
        }

        [Test]
        public void TestReceiveInDataReceive()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            bool triggered = false;
            client.OnDataReceive += async (sender, message) =>
            {
                var receivedData = message.Client.ReceiveAsync();
                message.Client.Send("Data");
                triggered = (await receivedData).ToString() == "Data";
            };
            
            client.Send("Echo");
            TestHelper.WaitWhileFalse(()=>triggered);
            Assert.IsTrue(triggered);
        }
    }
}
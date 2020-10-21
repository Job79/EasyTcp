using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.EasyTcpPacketUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client.LargeArray
{
    public class LargeArray
    {
        [Test]
        public void TestLargeArray()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += async (sender, message) =>
            {
                var array = await message.ReceiveLargeArrayAsync();
                message.Client.Send(array.Length);
                await message.Client.SendLargeArrayAsync(array);
            };
            
            byte[] receivedArray = null;

            using var client = new EasyTcpClient();
            client.OnDataReceive += (sender, message) => receivedArray = message.ReceiveLargeArray();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            client.Send("first message");

            byte[] largeMessage = new byte[ushort.MaxValue * 10];
            for (int i = 0; i < largeMessage.Length; i++) largeMessage[i] = 11;

            client.SendLargeArray(largeMessage);
            TestHelper.WaitWhileTrue(() => receivedArray == null);
            Assert.IsTrue(receivedArray.SequenceEqual(largeMessage));
        }
    }
}
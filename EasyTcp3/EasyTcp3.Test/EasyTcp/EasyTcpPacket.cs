using System.Net;
using System.Text;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp
{
    /*
     * Tests for EasyTcpPacket functions
     */
    public class EasyTcpPacket
    {
        [Test]
        public void PacketTest()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            var testData = new ExamplePacket("Test data");
            int received = 0;
            client.OnDataReceive += (sender, message) =>
            {
                string data = message.ToPacket<ExamplePacket>().DataStr;
                if (data.Equals(testData.DataStr)) Interlocked.Increment(ref received);
            };

            server.SendAll(testData);
            TestHelper.WaitWhileFalse(() => received == 1);
            Assert.AreEqual(1, received);
        }
    }

    class ExamplePacket : IEasyTcpPacket
    {
        public string DataStr;

        public byte[] Data
        {
            get => Encoding.UTF32.GetBytes(DataStr);
            set => DataStr = Encoding.UTF32.GetString(value);
        }

        public ExamplePacket()
        {
        }

        public ExamplePacket(string data) => DataStr = data;
    }
}
using System.Net;
using System.Text;
using System.Threading;
using EasyTcp3.ClientUtils;
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
                if (message.ToPacket<ExamplePacket>().Data == testData.Data) Interlocked.Increment(ref received);
            };
            
            server.SendAll(testData);
            TestHelper.WaitWhileFalse(()=> received == 1);
            Assert.AreEqual(1,received);
        }
    }

    class ExamplePacket : IEasyTcpPacket
    {
        public string Data { get; private set;  }

        public ExamplePacket(){}
        public ExamplePacket(string data) => Data = data;
        public byte[] ToArray() => Encoding.UTF32.GetBytes(Data);
        public void FromArray(byte[] data) => Data = Encoding.UTF32.GetString(data);
    }
}
using System.Linq;
using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.PacketUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp
{
    /// <summary>
    /// Tests for the Message functions
    /// </summary>
    public class Message
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer().Start(_port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message.Data);
        }

        [Test]
        public void ReceiveArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.SequenceEqual(receivedMessage.Data)) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveStringCompressed()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "r23235fdgbs23rvcfqarvgfagfrewfFD";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.Equals(receivedMessage.Decompress().ToString())) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data, compression: true);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }
    }
}

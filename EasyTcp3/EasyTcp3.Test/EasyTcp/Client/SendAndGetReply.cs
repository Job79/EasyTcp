using System;
using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for the SendAndGetReply functions
    /// </summary>
    public class SendAndGetReply
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup() // Simple echo server
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer().Start(_port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message.Data);
        }

        [Test]
        public void SendAndGetReplyTriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += (sender, message) => triggered = true;

            byte[] data = new byte[100];
            var m = client.SendAndGetReply(data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsFalse(triggered);
        }

        [Test]
        public void SendAndGetReplyInsideOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += (sender, message) =>
            {
                message.Client.Protocol.EnsureDataReceiverIsRunning(message.Client);
                var reply = message.Client.SendAndGetReply("ECHO"); 
                triggered = reply.ToString() == "ECHO";
            };

            client.Send("test");
            TestHelper.WaitWhileFalse(()=>triggered);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void SendAndGetReplyArrayWithoutTimeout()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = client.SendAndGetReply(data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
        }

        [Test]
        public void SendAndGetReplyArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = client.SendAndGetReply(data, _timeout);
            Assert.IsTrue(data.SequenceEqual(m.Data));
        }

        [Test]
        public void SendAndGetReplyString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToString());
        }
    }
}

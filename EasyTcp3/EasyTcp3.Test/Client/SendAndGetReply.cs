using System;
using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
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
            var server = new EasyTcpServer();
            server.Start(_port);
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
        public void SendAndGetReplyUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToUShort());
        }

        [Test]
        public void SendAndGetReplyShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToShort());
        }

        [Test]
        public void SendAndGetReplyUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToUInt());
        }

        [Test]
        public void SendAndGetReplyInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToInt());
        }

        [Test]
        public void SendAndGetReplyULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToULong());
        }

        [Test]
        public void SendAndGetReplyLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToLong());
        }

        [Test]
        public void SendAndGetReplyDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(data, m.ToDouble());
        }

        [Test]
        public void SendAndGetReplyBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool data = true;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var m = client.SendAndGetReply(data, _timeout);
            Assert.AreEqual(true, m.ToBool());
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
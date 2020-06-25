using System;
using System.Linq;
using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests for all the SendActionAndGetReply functions
    /// </summary>
    public class SendActionAndGetReply
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpActionServer().Start(_port);
        }

        [Test]
        public void SendActionAndGetReplyTriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += (sender, message) => triggered = true;

            byte[] data = new byte[100];
            var m = client.SendActionAndGetReply(0, data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsFalse(triggered);
        }

        [Test]
        public void SendActionAndGetReplyArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = client.SendActionAndGetReply(0, data);
            Assert.IsTrue(data.SequenceEqual(m.Data));

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.IsTrue(data.SequenceEqual(m2.Data));
        }

        [Test]
        public void SendActionAndGetReplyUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToUShort());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToUShort());
        }

        [Test]
        public void SendActionAndGetReplyShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToShort());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToShort());
        }

        [Test]
        public void SendActionAndGetReplyUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToUInt());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToUInt());
        }

        [Test]
        public void SendActionAndGetReplyInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToInt());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToInt());
        }

        [Test]
        public void SendActionAndGetReplyULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToULong());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToULong());
        }

        [Test]
        public void SendActionAndGetReplyLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToLong());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToLong());
        }

        [Test]
        public void SendActionAndGetReplyDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToDouble());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToDouble());
        }

        [Test]
        public void SendActionAndGetReplyBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            var m = client.SendActionAndGetReply(0, true, _timeout);
            Assert.AreEqual(true, m.ToBool());

            var m2 = client.SendActionAndGetReply("ECHO", true);
            Assert.AreEqual(true, m2.ToBool());
        }

        [Test]
        public void SendActionAndGetReplyString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            var m = client.SendActionAndGetReply(0, data, _timeout);
            Assert.AreEqual(data, m.ToString());

            var m2 = client.SendActionAndGetReply("ECHO", data);
            Assert.AreEqual(data, m2.ToString());
        }
    }
}
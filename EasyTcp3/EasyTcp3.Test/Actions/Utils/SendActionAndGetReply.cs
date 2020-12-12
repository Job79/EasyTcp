using System;
using System.Linq;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions.Utils
{
    public class SendActionAndGetReply
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(2);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpActionServer().Start(_port);
            // Detect actions from Actions.cs
        }

        [Test]
        public void SendActionAndGetReply_TriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            bool triggered = false;
            byte[] data = new byte[100];

            client.OnDataReceive += (sender, message) => triggered = true;
            client.SendActionAndGetReply(0, data);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void SendActionAndGetReply_Array()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            byte[] data = new byte[100];

            var m = client.SendActionAndGetReply(0, data);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsTrue(data.SequenceEqual(m2.Data));
        }

        [Test]
        public void SendActionAndGetReply_UShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            ushort data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToUShort());
            Assert.AreEqual(data, m2.ToUShort());
        }

        [Test]
        public void SendActionAndGetReply_Short()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            short data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToShort());
            Assert.AreEqual(data, m2.ToShort());
        }

        [Test]
        public void SendActionAndGetReply_UInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            uint data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToUInt());
            Assert.AreEqual(data, m2.ToUInt());
        }

        [Test]
        public void SendActionAndGetReply_Int()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            int data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToInt());
            Assert.AreEqual(data, m2.ToInt());
        }

        [Test]
        public void SendActionAndGetReply_ULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            ulong data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToULong());
            Assert.AreEqual(data, m2.ToULong());
        }

        [Test]
        public void SendActionAndGetReply_Long()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            long data = 123;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToLong());
            Assert.AreEqual(data, m2.ToLong());
        }

        [Test]
        public void SendActionAndGetReply_Double()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            double data = 123.0;

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToDouble());
            Assert.AreEqual(data, m2.ToDouble());
        }

        [Test]
        public void SendActionAndGetReply_Bool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));

            var m = client.SendActionAndGetReply(0, true, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", true);

            Assert.AreEqual(true, m.ToBool());
            Assert.AreEqual(true, m2.ToBool());
        }

        [Test]
        public void SendActionAndGetReply_String()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            string data = "123";

            var m = client.SendActionAndGetReply(0, data, _timeout);
            var m2 = client.SendActionAndGetReply("ECHO", data);

            Assert.AreEqual(data, m.ToString());
            Assert.AreEqual(data, m2.ToString());
        }
    }
}

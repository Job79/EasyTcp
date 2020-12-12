using System;
using System.Linq;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils.Async;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Actions.Utils
{
    public class SendActionAndGetReplyAsync
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpActionServer().Start(_port);
            // Detect actions from Actions.cs
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_TriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            bool triggered = false;
            byte[] data = new byte[100];

            client.OnDataReceive += (sender, message) => triggered = true;
            var m = await client.SendActionAndGetReplyAsync(0, data);

            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsFalse(triggered);
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Array()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            byte[] data = new byte[100];

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsTrue(data.SequenceEqual(m2.Data));
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_UShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            ushort data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToUShort());
            Assert.AreEqual(data, m2.ToUShort());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Short()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            short data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToShort());
            Assert.AreEqual(data, m2.ToShort());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_UInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            uint data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToUInt());
            Assert.AreEqual(data, m2.ToUInt());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Int()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            int data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToInt());
            Assert.AreEqual(data, m2.ToInt());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_ULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            ulong data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToULong());
            Assert.AreEqual(data, m2.ToULong());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Long()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            long data = 123;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToLong());
            Assert.AreEqual(data, m2.ToLong());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Double()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            double data = 123.0;

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToDouble());
            Assert.AreEqual(data, m2.ToDouble());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_Bool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));

            var m = await client.SendActionAndGetReplyAsync(0, true, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", true);

            Assert.AreEqual(true, m.ToBool());
            Assert.AreEqual(true, m2.ToBool());
        }

        [Test]
        public async Task SendActionAndGetReplyAsync_String()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", _port));
            string data = "123";

            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);

            Assert.AreEqual(data, m.ToString());
            Assert.AreEqual(data, m2.ToString());
        }
    }
}

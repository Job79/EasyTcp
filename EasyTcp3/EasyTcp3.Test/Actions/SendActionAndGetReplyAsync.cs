using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils.Async;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests for the SendActionAndGetReplyAsync functions
    /// </summary>
    public class SendActionAndGetReplyAsync
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpActionServer();
            server.Start(_port);
            //See action in Actions.cs
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncTriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += (sender, message) => triggered = true;

            byte[] data = new byte[100];
            var m = await client.SendActionAndGetReplyAsync(0, data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsFalse(triggered);
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.IsTrue(data.SequenceEqual(m.Data));

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.IsTrue(data.SequenceEqual(m2.Data));
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToUShort());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToUShort());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToShort());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToShort());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToUInt());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToUInt());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToInt());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToInt());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToULong());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToULong());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToLong());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToLong());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToDouble());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToDouble());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            var m = await client.SendActionAndGetReplyAsync(0, true, _timeout);
            Assert.AreEqual(true, m.ToBool());

            var m2 = await client.SendActionAndGetReplyAsync("ECHO", true);
            Assert.AreEqual(true, m2.ToBool());
        }

        [Test]
        public async Task SendActionAndGetReplyAsyncString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            var m = await client.SendActionAndGetReplyAsync(0, data, _timeout);
            Assert.AreEqual(data, m.ToString());
            
            var m2 = await client.SendActionAndGetReplyAsync("ECHO", data);
            Assert.AreEqual(data, m2.ToString());
        }
    }
}
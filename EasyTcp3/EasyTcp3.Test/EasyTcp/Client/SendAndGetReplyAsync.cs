using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for the SendAndGetReplyAsync functions
    /// </summary>
    public class SendAndGetReplyAsync
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(_port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message.Data);
        }

        [Test]
        public async Task SendAndGetReplyTriggerOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += (sender, message) => triggered = true;

            byte[] data = new byte[100];
            var m = await client.SendAndGetReplyAsync(data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
            Assert.IsFalse(triggered);
        }

        [Test]
        public async Task SendAndGetReplyArrayWithoutTimeout()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = await client.SendAndGetReplyAsync(data);
            Assert.IsTrue(data.SequenceEqual(m.Data));
        }

        [Test]
        public async Task SendAndGetReplyArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.IsTrue(data.SequenceEqual(m.Data));
        }

        [Test]
        public async Task SendAndGetReplyUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToUShort());
        }

        [Test]
        public async Task SendAndGetReplyShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToShort());
        }

        [Test]
        public async Task SendAndGetReplyUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToUInt());
        }

        [Test]
        public async Task SendAndGetReplyInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToInt());
        }

        [Test]
        public async Task SendAndGetReplyULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToULong());
        }

        [Test]
        public async Task SendAndGetReplyLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToLong());
        }

        [Test]
        public async Task SendAndGetReplyDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToDouble());
        }

        [Test]
        public async Task SendAndGetReplyBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool data = true;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(true, m.ToBool());
        }

        [Test]
        public async Task SendAndGetReplyString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToString());
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
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
        public async Task SendAndGetReplyArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.IsTrue(data.SequenceEqual(m.Data));
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.IsTrue(data.SequenceEqual(m2.Data));
        }

        [Test]
        public async Task SendAndGetReplyUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToUShort());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToUShort());
        }

        [Test]
        public async Task SendAndGetReplyShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToShort());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToShort());
        }

        [Test]
        public async Task SendAndGetReplyUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToUInt());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToUInt());
        }

        [Test]
        public async Task SendAndGetReplyInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToInt());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToInt());
        }

        [Test]
        public async Task SendAndGetReplyULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToULong());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToULong());
        }

        [Test]
        public async Task SendAndGetReplyLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToLong());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToLong());
        }

        [Test]
        public async Task SendAndGetReplyDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToDouble());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToDouble());
        }

        [Test]
        public async Task SendAndGetReplyBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool data = true;
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToBool());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToBool());
        }

        [Test]
        public async Task SendAndGetReplyString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            var m = await client.SendAndGetReplyAsync(data, _timeout);
            Assert.AreEqual(data, m.ToString());
            
            var m2 = await client.SendAndGetReplyAsync(data);
            Assert.AreEqual(data, m2.ToString());
        }
    }
}
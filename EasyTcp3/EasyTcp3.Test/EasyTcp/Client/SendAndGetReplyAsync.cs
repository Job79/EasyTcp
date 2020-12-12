using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for all the SendAndGetReplyAsync functions
    /// </summary>
    public class SendAndGetReplyAsync
    {
        private ushort _port;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer().Start(_port);
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
        public void SendAndGetReplyInsideOnDataReceive()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool triggered = false;
            client.OnDataReceive += async (sender, message) =>
            {
                var reply = await message.Client.SendAndGetReplyAsync("ECHO"); 
                triggered = reply.ToString() == "ECHO";
            };

            client.Send("test");
            TestHelper.WaitWhileFalse(()=>triggered);
            Assert.IsTrue(triggered);
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

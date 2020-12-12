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

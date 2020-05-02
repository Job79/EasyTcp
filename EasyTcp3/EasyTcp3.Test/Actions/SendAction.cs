using System.Net;
using System.Text;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests for the SendAction functions
    /// </summary>
    public class SendAction
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(_port);
        }

        [Test]
        public void SendActionArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            client.SendAction(0, data);
        }

        [Test]
        public void SendActionBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            client.SendAction(0, true);
        }

        [Test]
        public void SendActionString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "Data";
            client.SendAction(0, data);
            client.SendAction(0, data, Encoding.UTF32); //Send with different encoding
        }
    }
}
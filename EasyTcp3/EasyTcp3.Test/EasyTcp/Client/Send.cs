using System.Net;
using System.Text;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for all the Send functions
    /// </summary>
    public class Send
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
        public void SendArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            client.Send(data);
        }

        [Test]
        public void SendUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            client.Send(data);
        }

        [Test]
        public void SendShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            client.Send(data);
        }

        [Test]
        public void SendUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            client.Send(data);
        }

        [Test]
        public void SendInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            client.Send(data);
        }

        [Test]
        public void SendULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            client.Send(data);
        }

        [Test]
        public void SendLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            client.Send(data);
        }

        [Test]
        public void SendDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            client.Send(data);
        }

        [Test]
        public void SendBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool data = true;
            client.Send(data);
        }

        [Test]
        public void SendString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "Data";
            client.Send(data);
            client.Send(data, Encoding.UTF32); //Send with different encoding
        }
    }
}
using System.Linq;
using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp
{
    /// <summary>
    /// Tests for the Message functions
    /// </summary>
    public class Message
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(_port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message.Data);
        }

        [Test]
        public void ReceiveArray()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            byte[] data = new byte[100];
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.SequenceEqual(receivedMessage.Data)) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveUShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ushort data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidUShort() && data.Equals(receivedMessage.ToUShort()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveShort()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            short data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidShort() && data.Equals(receivedMessage.ToShort()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveUInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            uint data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidUInt() && data.Equals(receivedMessage.ToUInt()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveInt()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            int data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidInt() && data.Equals(receivedMessage.ToInt()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveULong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            ulong data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidULong() && data.Equals(receivedMessage.ToULong()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveLong()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            long data = 123;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidLong() && data.Equals(receivedMessage.ToLong()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveDouble()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            double data = 123.0;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidDouble() && data.Equals(receivedMessage.ToDouble()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveBool()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            bool data = true;
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (receivedMessage.IsValidBool() && data.Equals(receivedMessage.ToBool()))
                    Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void ReceiveString()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }
        
        [Test]
        public void ReceiveStringCompressed()
        {
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, _port));

            string data = "123123123123123123123123123123123123";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (data.Equals(receivedMessage.Decompress().ToString())) Interlocked.Increment(ref receiveCount);
            };

            client.Send(data, compression: true);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }
    }
}
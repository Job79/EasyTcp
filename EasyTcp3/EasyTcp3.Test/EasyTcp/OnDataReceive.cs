using System;
using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp
{
    /// <summary>
    /// Tests for the OnDataReceive event
    /// </summary>
    public class OnDataReceive
    {
        [Test]
        public void OnDataReceive1()
        {
            // Example receiving for the server
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            const string message = "Hello server!";
            int receiveCount = 0;

            server.OnDataReceive += (s, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
                Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
            };

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Send(message);

            TestHelper.WaitWhileTrue(() => receiveCount < 1);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void OnDataReceive2()
        {
            // Example receiving for the client
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            const string message = "Hello client!";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, so thread safe increase integer
                if (message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
                Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
            };

            server.SendAll(message);

            TestHelper.WaitWhileTrue(() => receiveCount < 1);
            Assert.AreEqual(1, receiveCount);
        }
    }
}
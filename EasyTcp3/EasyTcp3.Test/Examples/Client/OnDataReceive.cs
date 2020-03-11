using System;
using System.Net;
using System.Threading;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples
{
    public class OnDataReceive
    {
        [Test]
        public void OnDataReceive1()
        {
            // Example receiving for the server
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            const string message = "Hello server!";
            int receiveCount = 0;

            server.OnConnect += (sender, client) =>
            {
                client.OnDataReceive += (sender, receivedMessage) =>
                {
                    //Async lambda, thread safe increase integer
                    if (message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
                    Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
                };
                Console.WriteLine("Client connected");
            };

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Send(message);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }

        [Test]
        public void OnDataReceive2()
        {
            // Example receiving for the client
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            const string message = "Hello server!";
            int receiveCount = 0;

            client.OnDataReceive += (sender, receivedMessage) =>
            {
                //Async lambda, thread safe increase integer
                if(message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount); 
                Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
            };
            
            server.SendAll(message);

            TestHelper.WaitWhileTrue(() => receiveCount == 0);
            Assert.AreEqual(1, receiveCount);
        }
    }
}
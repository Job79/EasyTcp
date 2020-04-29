using System;
using System.IO;
using System.Net;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
    public static class Stream
    {
        [Test]
        public static void Stream1() //Client -> -(Stream)> Server     (Client sends message to server)
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            
            string testData = "123", data = null;

            server.OnDataReceive += (sender, message) => //Receive stream from client
            {
                using var stream = new MemoryStream();
                message.ReceiveStream(stream);
                data = Encoding.UTF8.GetString(stream.ToArray());
            };

            //Send stream to server
            using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(testData));
            client.Send("Stream");
            client.SendStream(dataStream);

            TestHelper.WaitWhileTrue(() => data == null);
            Assert.AreEqual(testData, data);
        }

        [Test]
        public static void Stream2()//Client -> Server -(Stream)> Client     (Client requests stream from server)
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            string testData = "123", data = null;

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            server.OnDataReceive += (sender, message) => //Send stream if client requests
            {
                using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(testData));
                message.Client.Send("Stream");
                message.Client.SendStream(dataStream);
            };
            
            client.OnDataReceive += (sender, message) => //Receive stream from server
            {
                using var stream = new MemoryStream();
                message.ReceiveStream(stream);
                data = Encoding.UTF8.GetString(stream.ToArray());
            };
            client.Send("GetStream");//Request stream

            TestHelper.WaitWhileTrue(() => data == null);
            Assert.AreEqual(testData, data);
        }
    }
}
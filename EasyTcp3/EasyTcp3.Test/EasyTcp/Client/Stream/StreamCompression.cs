using System.IO;
using System.Net;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Client.Stream
{
    public class StreamCompression
    {
        [Test]
        public void Stream1() //Client -> -(Stream)> Server     (Client sends message to server)
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            string testData = "123", data = null;

            server.OnDataReceive += (sender, message) => //Receive stream from client
            {
                using var stream = new MemoryStream();
                message.ReceiveStream(stream, compression: true);
                data = Encoding.UTF8.GetString(stream.ToArray());
            };

            //Send stream to server
            using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(testData));
            client.Send("Stream");
            client.SendStream(dataStream, compression: true);

            TestHelper.WaitWhileTrue(() => data == null);
            Assert.AreEqual(testData, data);
        }
    }
}
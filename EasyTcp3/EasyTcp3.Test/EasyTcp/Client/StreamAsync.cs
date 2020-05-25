using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for the StreamAsync functions
    /// </summary>
    public class StreamAsync
    {
        //TODO This test crashes sometimes, is it the test or the code in EasyTcp?
        [Test]
        public async Task Stream1() //Client -> -(Stream)> Server     (Client sends message to server)
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            
            string testData = "123", data = "";

            server.OnDataReceive += (sender, message) => //Receive stream from client
            {
                using var stream = new MemoryStream();
                message.ReceiveStreamAsync(stream).Wait();
                data = Encoding.UTF8.GetString(stream.ToArray());
            };

            //Send stream to server
            await using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(testData));
            client.Send("Stream");
            await client.SendStreamAsync(dataStream);

            TestHelper.WaitWhileTrue(() => data == "");
            Assert.AreEqual(testData, data);
        }

        [Test]
        public void Stream2()//Client -> Server -(Stream)> Client     (Client requests stream from server)
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
                message.Client.SendStreamAsync(dataStream).Wait();
            };
            
            client.OnDataReceive += async (sender, message) => //Receive stream from server
            {
                await using var stream = new MemoryStream();
                await message.ReceiveStreamAsync(stream);
                data = Encoding.UTF8.GetString(stream.ToArray());
            };
            client.Send("GetStream");//Request stream

            TestHelper.WaitWhileTrue(() => data == null);
            Assert.AreEqual(testData, data);
        }
    }
}


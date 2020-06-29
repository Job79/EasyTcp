using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    public class StreamSsl
    {
        [Test]
        public void Stream()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().UseSsl(certificate).Start(port);

            using var client = new EasyTcpClient().UseSsl("localhost",true);
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
    }
}
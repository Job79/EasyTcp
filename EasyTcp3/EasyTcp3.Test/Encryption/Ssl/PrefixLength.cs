using System.Security.Cryptography.X509Certificates;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    /// <summary>
    /// Tests for PrefixLengthSslProtocol
    /// </summary>
    public class PrefixLength
    {
        [Test]
        public void TestSending()
        {
            ushort port = TestHelper.GetPort();
            var certificate = new X509Certificate2("certificate.pfx", "password");
            using var protocol = new PrefixLengthSslProtocol(certificate);
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message)
                => message.Client.Send(message);

            using var cprotocol = new PrefixLengthSslProtocol("localhost", true);
            using var client = new EasyTcpClient(cprotocol);
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var message = client.SendAndGetReply("Test");
            Assert.AreEqual("Test", message.ToString());
        }
    }
}
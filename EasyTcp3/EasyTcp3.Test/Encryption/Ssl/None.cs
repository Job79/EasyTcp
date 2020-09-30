using System.Security.Cryptography.X509Certificates;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption.Protocols.Tcp.Ssl;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.Encryption.Ssl
{
    /// <summary>
    /// Tests for NoneSslProtocol
    /// </summary>
    public class None
    {
        [Test]
        public void TestSending()
        {
            ushort port = TestHelper.GetPort();
            var certificate = new X509Certificate2("certificate.pfx", "password");
            using var protocol = new NoneSslProtocol(certificate);
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message)
                => message.Client.Send(message);

            using var cprotocol = new NoneSslProtocol("localhost", acceptInvalidCertificates: true);
            using var client = new EasyTcpClient(cprotocol);
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var message = client.SendAndGetReply("Test");
            Assert.AreEqual("Test", message.ToString());
        } 
    }
}
using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    public class Ssl
    {
        [Test]
        public void TestInvalidCertificate()
        {
            ushort port = TestHelper.GetPort();
            var certificate = new X509Certificate2("test.pfx", "password");
            using var protocol = new PrefixLengthSslProtocol(certificate);
            using var server = new EasyTcpServer(protocol).Start(port);

            using var cprotocol = new PrefixLengthSslProtocol("localhost");
            using var client = new EasyTcpClient(cprotocol);
            Assert.IsFalse(client.Connect("127.0.0.1", port));
        }
    }
}
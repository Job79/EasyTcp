using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    /// <summary>
    /// Tests for PrefixLengthSslProtocol 
    /// </summary>
    public class Ssl
    {
        [Test]
        public void TestInvalidCertificate()
        {
            ushort port = TestHelper.GetPort();
            var certificate = new X509Certificate2("certificate.pfx", "password");
            using var server = new EasyTcpServer().UseSsl(certificate).Start(port);

            using var client = new EasyTcpClient().UseSsl("localhost");
            Assert.IsFalse(client.Connect("127.0.0.1", port));
        }
    }
}
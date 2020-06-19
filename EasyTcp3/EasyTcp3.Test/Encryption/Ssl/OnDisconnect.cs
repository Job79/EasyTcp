using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using EasyTcp.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    public class OnDisconnect
    {
        [Test]
        public void OnDisconnectServer()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            using var protocol = new PrefixLengthSslProtocol(certificate);
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer(protocol);
            server.Start(IPAddress.Any, port);

            int x = 0;
            server.OnDisconnect += (o, c) => Interlocked.Increment(ref x);

            using var cprotocol = new PrefixLengthSslProtocol("localhost", true);
            var client = new EasyTcpClient(cprotocol);
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
    }
}
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    /// <summary>
    /// Tests for the OnDisconnect event
    /// </summary>
    public class OnDisconnect
    {
        [Test]
        public void OnDisconnectServer()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().UseSsl(certificate).Start(port);

            int x = 0;
            server.OnDisconnect += (o, c) => Interlocked.Increment(ref x);

            var client = new EasyTcpClient().UseSsl("localhost", true);
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            client.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
        
        [Test]
        public void OnDisconnectClient()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer().UseSsl(certificate).Start(port);

            var client = new EasyTcpClient().UseSsl("localhost", true);
            int x = 0;
            client.OnDisconnect += (o, c) => Interlocked.Increment(ref x);

            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            server.Dispose();

            TestHelper.WaitWhileFalse(() => x == 1);
            Assert.AreEqual(1, x);
        }
    }
}
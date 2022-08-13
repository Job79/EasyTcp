using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Encryption.Ssl;
using NUnit.Framework;

namespace EasyTcp4.Test.Encryption.Ssl.Protocols
{
    public class PlainTcp
    {
        [Test]
        public async Task PlainSslProtocolReceiveData()
        {
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PlainSslProtocol("localhost", true)),
                    new EasyTcpServer(new PlainSslProtocol(certificate)));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000]);

            await TestHelper.WaitWhileFalse(() => receivedBytes == 1000);
            Assert.AreEqual(1000, receivedBytes);
        }

        [Test]
        public async Task PlainSslProtocolReceiveDataContent()
        {
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PlainSslProtocol("localhost", true)),
                    new EasyTcpServer(new PlainSslProtocol(certificate)));


            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, short.MaxValue * 2).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == short.MaxValue * 2);
            Assert.AreEqual(short.MaxValue * 2, receivedBytes);
        }

        [Test]
        public async Task PlainSslProtocolReceiveLargeData()
        {
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var conn = await TestHelper.GetTestConnection(
                new EasyTcpClient(new PlainSslProtocol("localhost", true)),
                new EasyTcpServer(new PlainSslProtocol(certificate)));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x => x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 2).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 2).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 2).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == ushort.MaxValue * 6);
            Assert.AreEqual(ushort.MaxValue * 6, receivedBytes);
        }
    }
}

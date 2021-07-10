using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Encryption;
using EasyTcp4.ClientUtils.Async;
using NUnit.Framework;

namespace EasyTcp4.Test.Encryption.Ssl
{
    public class StreamUtil
    {
        [Test]
        public async Task SendStreamAsync()
        {
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient().UseSsl("localhost", true),
                    new EasyTcpServer().UseServerSsl(certificate));

            long receivedBytes = 0;
            conn.Server.OnDataReceiveAsync += async (_, m) => 
            {
                using var stream = new MemoryStream();
                await m.ReceiveStreamAsync(stream);
                Interlocked.Add(ref receivedBytes, stream.Length);
            };

            using var stream = new MemoryStream(new byte[ushort.MaxValue * 5]);
            conn.Client.Send("Trigger OnDataReceive");
            await conn.Client.SendStreamAsync(stream);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == stream.Length);
            Assert.AreEqual(stream.Length, receivedBytes);
        }
    }
}

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Encryption;
using EasyTcp4.ClientUtils.Async;
using NUnit.Framework;

namespace EasyTcp4.Test.Encryption.Ssl
{
    public class ArrayUtil
    {
        [Test]
        public async Task SendArrayAsync()
        {
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient().UseSsl("localhost", true),
                    new EasyTcpServer().UseServerSsl(certificate));

            int receivedBytes = 0;
            conn.Server.OnDataReceiveAsync += async (_, m) => 
            {
                var data = await m.ReceiveArrayAsync();
                Interlocked.Add(ref receivedBytes, data.Length);
            };

            conn.Client.Send("Trigger OnDataReceive");
            await conn.Client.SendArrayAsync(new byte[ushort.MaxValue * 5]);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == ushort.MaxValue * 5);
            Assert.AreEqual(ushort.MaxValue * 5, receivedBytes);
        }
    }
}

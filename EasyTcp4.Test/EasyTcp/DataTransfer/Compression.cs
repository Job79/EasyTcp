using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using NUnit.Framework;
using EasyTcp4.PacketUtils;

namespace EasyTcp4.Test.EasyTcp.DataTransfer
{
    public class Compression
    {
        [Test]
        public async Task SendCompressedArray()
        {
            using var conn = await TestHelper.GetTestConnection();
            
            int receivedBytes = 0, receivedBytesDecompressed = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytesDecompressed, m.Decompress().Data.Length);
            conn.Client.Send(new byte[1000], compression: true);

            await TestHelper.WaitWhileFalse(()=>receivedBytesDecompressed == 1000);
            Assert.IsTrue(receivedBytesDecompressed > receivedBytes);
            Assert.AreEqual(1000, receivedBytesDecompressed);
        }
    }
}

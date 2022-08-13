using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.ClientUtils.Async;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Utils
{
    public class StreamUtil
    {
        [Test]
        public async Task SendStream()
        {
            using var conn = await TestHelper.GetTestConnection();

            long receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => 
            {
                using var stream = new MemoryStream();
                m.ReceiveStream(stream);
                Interlocked.Add(ref receivedBytes, stream.Length);
            };

            using var stream = new MemoryStream(new byte[ushort.MaxValue * 5]);
            conn.Client.Send("Trigger OnDataReceive");
            conn.Client.SendStream(stream);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == stream.Length);
            Assert.AreEqual(stream.Length, receivedBytes);
        }

        [Test]
        public async Task SendCompressedStream()
        {
            using var conn = await TestHelper.GetTestConnection();

            long receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => 
            {
                using var stream = new MemoryStream();
                m.ReceiveStream(stream, compression: true);
                Interlocked.Add(ref receivedBytes, stream.Length);
            };

            using var stream = new MemoryStream(new byte[ushort.MaxValue * 5]);
            conn.Client.Send("Trigger OnDataReceive");
            conn.Client.SendStream(stream, compression: true);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == stream.Length);
            Assert.AreEqual(stream.Length, receivedBytes);
        }

        [Test]
        public async Task SendStreamAsync()
        {
            using var conn = await TestHelper.GetTestConnection();

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

        [Test]
        public async Task SendCompressedStreamAsync()
        {
            using var conn = await TestHelper.GetTestConnection();

            long receivedBytes = 0;
            conn.Server.OnDataReceiveAsync += async (_, m) => 
            {
                using var stream = new MemoryStream();
                await m.ReceiveStreamAsync(stream, compression: true);
                Interlocked.Add(ref receivedBytes, stream.Length);
            };

            using var stream = new MemoryStream(new byte[ushort.MaxValue * 5]);
            conn.Client.Send("Trigger OnDataReceive");
            await conn.Client.SendStreamAsync(stream, compression: true);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == stream.Length);
            Assert.AreEqual(stream.Length, receivedBytes);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.ClientUtils.Async;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.Utils
{
    public class ArrayUtil
    {
        [Test]
        public async Task SendArray()
        {
            using var conn = await TestHelper.GetTestConnection();

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => 
            {
                var data = m.ReceiveArray();
                Interlocked.Add(ref receivedBytes, data.Length);
            };

            conn.Client.Send("Trigger OnDataReceive");
            conn.Client.SendArray(new byte[ushort.MaxValue * 5]);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == ushort.MaxValue * 5);
            Assert.AreEqual(ushort.MaxValue * 5, receivedBytes);
        }

        [Test]
        public async Task SendCompressedArray()
        {
            using var conn = await TestHelper.GetTestConnection();

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => 
            {
                var data = m.ReceiveArray(compression: true);
                Interlocked.Add(ref receivedBytes, data.Length);
            };

            conn.Client.Send("Trigger OnDataReceive");
            conn.Client.SendArray(new byte[ushort.MaxValue * 5], compression: true);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == ushort.MaxValue * 5);
            Assert.AreEqual(ushort.MaxValue * 5, receivedBytes);
        }

        [Test]
        public async Task SendArrayAsync()
        {
            using var conn = await TestHelper.GetTestConnection();

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

        [Test]
        public async Task SendCompressedArrayAsync()
        {
            using var conn = await TestHelper.GetTestConnection();

            int receivedBytes = 0;
            conn.Server.OnDataReceiveAsync += async (_, m) => 
            {
                var data = await m.ReceiveArrayAsync(compression: true);
                Interlocked.Add(ref receivedBytes, data.Length);
            };

            conn.Client.Send("Trigger OnDataReceive");
            await conn.Client.SendArrayAsync(new byte[ushort.MaxValue * 5], compression: true);
            
            await TestHelper.WaitWhileFalse(()=> receivedBytes == ushort.MaxValue * 5);
            Assert.AreEqual(ushort.MaxValue * 5, receivedBytes);
        }
    }
}

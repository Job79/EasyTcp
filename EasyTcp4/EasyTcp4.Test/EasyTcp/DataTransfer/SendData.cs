using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.DataTransfer
{
    public class SendData
    {
        [Test]
        public async Task SendArray()
        {
            using var conn = await TestHelper.GetTestConnection();
            
            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000]);

            await TestHelper.WaitWhileFalse(()=>receivedBytes == 1000);
            Assert.AreEqual(1000, receivedBytes);
        }

        [Test]
        public async Task SendMultipleArrays()
        {
            using var conn = await TestHelper.GetTestConnection();
            
            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000], new byte[1000], new byte[1000]);

            await TestHelper.WaitWhileFalse(()=>receivedBytes == 3000);
            Assert.AreEqual(3000, receivedBytes);
        }

        [Test]
        public async Task SendString()
        {
            using var conn = await TestHelper.GetTestConnection();
            
            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send("test");

            await TestHelper.WaitWhileFalse(()=>receivedBytes == 4);
            Assert.AreEqual(4, receivedBytes);
        }
    }
}

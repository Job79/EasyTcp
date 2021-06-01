using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Protocols.Tcp;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.DataTransfer.Protocols
{
    public class PlainTcp
    {
        [Test]
        public async Task PlainTcpProtocolReceiveData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PlainTcpProtocol()),
                    new EasyTcpServer(new PlainTcpProtocol()));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000]);

            await TestHelper.WaitWhileFalse(() => receivedBytes == 1000);
            Assert.AreEqual(1000, receivedBytes);
        }

        [Test]
        public async Task PlainTcpProtocolReceiveDataContent()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PlainTcpProtocol()),
                    new EasyTcpServer(new PlainTcpProtocol()));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, short.MaxValue * 2).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == short.MaxValue * 2);
            Assert.AreEqual(short.MaxValue * 2, receivedBytes);
        }


        [Test]
        public async Task PlainTcpProtocolReceiveLargeData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PlainTcpProtocol()),
                    new EasyTcpServer(new PlainTcpProtocol()));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 128).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 128).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 128).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == ushort.MaxValue * 384);
            Assert.AreEqual(ushort.MaxValue * 384, receivedBytes);
        }
    }
}

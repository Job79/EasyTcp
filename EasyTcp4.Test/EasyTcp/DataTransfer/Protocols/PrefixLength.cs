using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Protocols.Tcp;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.DataTransfer.Protocols
{
    public class PrefixLength
    {
        [Test]
        public async Task PrefixLengthProtocolReceiveData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PrefixLengthProtocol()),
                    new EasyTcpServer(new PrefixLengthProtocol()));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000]);

            await TestHelper.WaitWhileFalse(() => receivedBytes == 1000);
            Assert.AreEqual(1000, receivedBytes);
        }

        [Test]
        public async Task PrefixLengthProtocolReceiveDataContent()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PrefixLengthProtocol()),
                    new EasyTcpServer(new PrefixLengthProtocol()));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, short.MaxValue * 2).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == short.MaxValue * 2);
            Assert.AreEqual(short.MaxValue * 2, receivedBytes);
        }


        [Test]
        public async Task PrefixLengthProtocolReceiveLargeData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new PrefixLengthProtocol(int.MaxValue)),
                    new EasyTcpServer(new PrefixLengthProtocol(int.MaxValue)));

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

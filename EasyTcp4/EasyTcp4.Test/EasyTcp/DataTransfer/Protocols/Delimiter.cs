using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using EasyTcp4.Protocols.Tcp;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.DataTransfer.Protocols
{
    public class Delimiter
    {
        private const string DelimiterString = "|";

        [Test]
        public async Task DelimiterProtocolReceiveData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new DelimiterProtocol(DelimiterString)),
                    new EasyTcpServer(new DelimiterProtocol(DelimiterString)));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Length);
            conn.Client.Send(new byte[1000]);

            await TestHelper.WaitWhileFalse(() => receivedBytes == 1000);
            Assert.AreEqual(1000, receivedBytes);
        }

        [Test]
        public async Task DelimiterProtocolReceiveDataContent()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new DelimiterProtocol(DelimiterString)),
                    new EasyTcpServer(new DelimiterProtocol(DelimiterString)));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, short.MaxValue * 2).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == short.MaxValue * 2);
            Assert.AreEqual(short.MaxValue * 2, receivedBytes);
        }


        [Test]
        public async Task DelimiterProtocolReceiveLargeData()
        {
            using var conn = await TestHelper.GetTestConnection(
                    new EasyTcpClient(new DelimiterProtocol(DelimiterString)),
                    new EasyTcpServer(new DelimiterProtocol(DelimiterString)));

            int receivedBytes = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedBytes, m.Data.Count(x=>x == 100));
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 8).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 8).ToArray());
            conn.Client.Send(Enumerable.Repeat<byte>(100, ushort.MaxValue * 8).ToArray());

            await TestHelper.WaitWhileFalse(() => receivedBytes == ushort.MaxValue * 24, TimeSpan.FromSeconds(5));
            Assert.AreEqual(ushort.MaxValue * 24, receivedBytes);
        }
    }
}

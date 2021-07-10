using System;
using System.Threading;
using EasyTcp4.ServerUtils;
using System.Net;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils.Async;

namespace EasyTcp4.Test
{
    public static class TestHelper
    {
        /// <summary>
        /// Unique port generater for every test
        /// </summary>
        private static int _portCounter = 1200;
        public static ushort GetPort() => (ushort)Math.Min(Interlocked.Increment(ref _portCounter), ushort.MaxValue);

        public static async Task<TestData> GetTestConnection(EasyTcpClient client = null, EasyTcpServer server = null)
        {
            var port = GetPort();
            server ??= new EasyTcpServer();
            client ??= new EasyTcpClient();
            server.Start(port);
            await client.ConnectAsync(IPAddress.Loopback, port);
            return new TestData() { Client = client, Server = server };
        }

        /// <summary>
        /// Wait until function refurns true
        /// </summary>
        public static async Task WaitWhileTrue(Func<bool> f, TimeSpan? timeout = null)
        {
            for (int i = 0; i < (timeout ?? TimeSpan.FromSeconds(1)).TotalMilliseconds && f(); i += 5)
                await Task.Delay(5);
        }

        /// <summary>
        /// Wait until function refurns false 
        /// </summary>
        public static Task WaitWhileFalse(Func<bool> f, TimeSpan? timeout = null) => WaitWhileTrue(() => !f(), timeout);
    }

    public class TestData : IDisposable
    {
        public EasyTcpClient Client;
        public EasyTcpServer Server;

        public void Dispose()
        {
            Client?.Dispose();
            Server?.Dispose();
        }
    }
}

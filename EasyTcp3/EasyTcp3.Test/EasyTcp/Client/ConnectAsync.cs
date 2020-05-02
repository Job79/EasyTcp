using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Client
{
    /// <summary>
    /// Tests for the ConnectAsync functions
    /// </summary>
    public class ConnectAsync
    {
        private ushort _port;

        [SetUp]
        public void Setup()
        {
            _port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(_port);
        }

        [Test]
        public async Task Connect1()
        {
            using var client = new EasyTcpClient();
            bool isConnected = await client.ConnectAsync(IPAddress.Any, _port);
            Assert.IsTrue(isConnected);
        }

        [Test]
        public async Task Connect2()
        {
            using var client = new EasyTcpClient();
            bool isConnected = await client.ConnectAsync("127.0.0.1", _port);
            Assert.IsTrue(isConnected);
        }
    }
}
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Server
{
    public class EventHandlers
    {
        [Test]
        public void OnClientConnectTest()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 1287);

            bool x = false;
            server.OnClientConnect += (_, socket) => x = true;
            client.Connect(IPAddress.Any, 1287);

            for (int i = 0; i < 1000 && !x; i++) Task.Delay(1).Wait();
            Assert.IsTrue(x, "Server did not fire OnClientConnect");
        }

        [Test]
        public void OnClientDisconnect()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 1281);

            bool x = false;
            server.OnClientDisconnect += (_, socket) => x = true;
            client.Connect(IPAddress.Any, 1281);
            client.Dispose();

            for (int i = 0; i < 1000 && !x; i++) Task.Delay(1).Wait();
            Assert.IsTrue(x, "Server did not fire OnClientDisconnect");
        }

        [Test]
        public void OnDataReceive()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 1282);
            client.Connect(IPAddress.Any, 1282);

            byte[] data = {12, 42, 12, 5, 12, 65, 32};
            bool x = false;
            server.OnDataReceive += (_, message) => x = data.SequenceEqual(message.Data);
            client.Send(data);

            for (int i = 0; i < 1000 && !x; i++) Task.Delay(1).Wait();
            Assert.IsTrue(x, "Server did not receive data from client");
        }

        /* OnError, not possible to test */
    }
}
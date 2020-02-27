using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.Client;
using EasyTcp3.Server;
using EasyTcp3.Server.Helpers;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
    public class EventHandlers
    {
        [Test]
        public void OnDisconnect()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 1563);
            client.Connect(IPAddress.Any, 1563);

            var x = false;
            client.OnDisconnect += (sender, c) => x = true;
            client.Dispose();

            for (int i = 0; i < 1000 && !x; i++) Task.Delay(1).Wait();
            Assert.IsTrue(x);
        }

        [Test]
        public void OnDataReceive()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 2001);
            client.Connect(IPAddress.Any, 2001);

            byte[] data = {12, 21, 76, 123, 75, 96};
            bool x = false;
            client.OnDataReceive += (sender, message) => x = data.SequenceEqual(message.Data);
            server.SendAll(data);

            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsTrue(x, "Client did not receive data from server");
        }

        [Test]
        public void OnClientConnect()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();

            server.Start(IPAddress.Any, 2133);

            var x = false;
            client.OnConnect += (sender, e) => x = true;

            client.Connect(IPAddress.Any, 2133);
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsTrue(x, "Connect event was not fired");
        }
    }
}
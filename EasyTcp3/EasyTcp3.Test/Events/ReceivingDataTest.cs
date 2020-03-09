using System.Linq;
using System.Net;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Events
{
    public class ReceivingDataTest
    {
        private static byte[] testData = {12, 42, 12, 43, 12, 53, 12, 64, 12, 87, 12};

        [Test]
        public void TestReceivingClient()
        {
            var port = TestHelper.GetPort();
            var client = new EasyTcpClient();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout), "Client did not connect");

            byte x = 0;
            client.OnDataReceive += (o, m) => x = m.Data.SequenceEqual(testData) ? (byte)2 : (byte)1;
            server.SendAll(testData);

            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(0,x,"Server did not receive any data");
            Assert.AreNotEqual(1,x,"Server did not receive correct data");
            Assert.AreEqual(2,x);
        }
        
        [Test]
        public void TestReceivingServer()
        {
            var port = TestHelper.GetPort();
            var client = new EasyTcpClient();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            byte x = 0;
            server.OnConnect += (o, client) =>
            {
                client.OnDataReceive += (o,m)=> x = m.Data.SequenceEqual(testData) ? (byte)2 : (byte)1;
            };
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout), "Client did not connect");
            client.Send(testData);

            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(0,x,"Server did not receive any data");
            Assert.AreNotEqual(1,x,"Server did not receive correct data");
            Assert.AreEqual(2,x);
        }
    }
}
using System.Net;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Events
{
    public class DisconnectTest
    {
        [Test]
        public void OnDisconnectClient()
        {
            var port = TestHelper.GetPort();
            var client = new EasyTcpClient();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            byte x = 0;
            client.OnDisconnect += (o, m) => x++;
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout), "Client did not connect");
            server.Stop();
            
            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(x,0,"Event was not fired");
            Assert.AreEqual(x,1);
        }
        
        [Test]
        public void OnDisconnectServer()
        {
            var port = TestHelper.GetPort();
            var client = new EasyTcpClient();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            byte x = 0;
            server.OnConnect += (o, client) => { client.OnDisconnect += (o, m) => x++; };
            Assert.IsTrue(client.Connect(IPAddress.Any, port, TestHelper.DefaultTimeout), "Client did not connect");
            server.Stop();
            
            TestHelper.Wait(()=>x != 0);
            Assert.AreNotEqual(0,x,"Event was not fired");
            Assert.AreEqual(1,x);
        }
    }
}
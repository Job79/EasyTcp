using System.Net;
using System.Threading;
using NUnit.Framework;
using EasyTcp3.Client;
using EasyTcp.Server;

namespace EasyTcp3.Test.Client
{
    public class ReceiveData
    {
        [Test]
        public void ReceiveDataTest()
        {
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, 2001, 10);
            
            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, 2001);
            
            bool x = false;
            client.OnDataReceive += (sender, message) => x = true;
            server.Broadcast(false);
            
            for (int i = 0; i < 1000 && !x; i++) Thread.Sleep(1);
            Assert.IsTrue(x,"Did not receive any data from server");
        }
    }
}
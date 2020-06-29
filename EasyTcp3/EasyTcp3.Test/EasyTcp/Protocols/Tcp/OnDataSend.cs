using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Protocols.Tcp
{
    public class OnDataSend
    {
        [Test]
        public void OnSendClient()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            
            int x = 0;
            client.OnDataSend += (_, message) => Interlocked.Increment(ref x);
            
            client.Send("Test");
            client.Send("Test");
            
            TestHelper.WaitWhileFalse(() => x == 2);
            Assert.AreEqual(2, x);
        }
        
        [Test]
        public void OnSendServer()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (_, message) => message.Client.Send(message);
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            
            int x = 0;
            server.OnDataSend += (_, message) => Interlocked.Increment(ref x);
            
            client.Send("Test");
            client.Send("Test");
            
            TestHelper.WaitWhileFalse(() => x == 2);
            Assert.AreEqual(2, x);
        }
    }
}
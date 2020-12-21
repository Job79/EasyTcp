using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using NUnit.Framework;
using EasyTcp3.ServerUtils;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3.Test.EasyTcp.Protocols.Tcp
{
    public class OnDataSend
    {
        [Test]
        public void OnSendClient()
        {
            var port = TestHelper.GetPort();
            using var server = new EasyTcpServer(new PrefixLengthProtocol(int.MaxValue)).Start(port);
            using var client = new EasyTcpClient(new PrefixLengthProtocol(int.MaxValue));
            Assert.IsTrue(client.Connect("127.0.0.1", port));
            
            int x = 0;
            server.OnDataReceive += (_, message) => Interlocked.Add(ref x, message.Data.Length);
            
            client.Send(new byte[ushort.MaxValue * 3]);
            client.Send(new byte[ushort.MaxValue * 10]);
            client.Send(new byte[ushort.MaxValue * 7]);
            
            TestHelper.WaitWhileFalse(() => x == ushort.MaxValue * 20);
            Assert.AreEqual(ushort.MaxValue * 20, x);
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

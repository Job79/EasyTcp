using EasyTcp3.ClientUtils;
using EasyTcp3.Protocols.Tcp;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Protocols.Tcp
{
    /// <summary>
    /// Tests for DelimiterProtocol
    /// </summary>
    public class Delimiter
    {
        [Test]
        public void TestReceivingAndSendingData()
        {
            ushort port = TestHelper.GetPort(); 
            var protocol = new DelimiterProtocol("\r\n");
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
            
            using var client = new EasyTcpClient(protocol);
            Assert.IsTrue(client.Connect("127.0.0.1",port));
            
            var data = "testMessage";
            var reply = client.SendAndGetReply(data);
            Assert.IsNotNull(reply);
            Assert.AreEqual(data,reply.ToString());
        }
        
        [Test]
        public void TestSplittingData()
        {
            ushort port = TestHelper.GetPort(); 
            var protocol = new DelimiterProtocol("\r\n",false,false);
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
            
            using var client = new EasyTcpClient(new DelimiterProtocol("\r\n", false));
            Assert.IsTrue(client.Connect("127.0.0.1",port));
            
            var data = "test\r\nMessage";
            var reply = client.SendAndGetReply(data);
            Assert.IsNotNull(reply);
            Assert.AreEqual("test",reply.ToString());
        }
    }
}
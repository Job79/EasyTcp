using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption.Ssl
{
    public class Delimiter
    {
        [Test]
        public void TestReceivingAndSendingData()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            
            ushort port = TestHelper.GetPort(); 
            var protocol = new DelimiterSslProtocol("\r\n", certificate);
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
            
            using var client = new EasyTcpClient(new DelimiterSslProtocol("\r\n", "localhost", acceptInvalidCertificates: true));
            Assert.IsTrue(client.Connect("127.0.0.1",port));
            
            var data = "testMessage";
            var reply = client.SendAndGetReply(data);
            Assert.IsNotNull(reply);
            Assert.AreEqual(data,reply.ToString());
        }
        
        [Test]
        public void TestSplittingData()
        {
            var certificate = new X509Certificate2("certificate.pfx", "password");
            
            ushort port = TestHelper.GetPort(); 
            var protocol = new DelimiterSslProtocol("\r\n", certificate);
            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
            
            using var client = new EasyTcpClient(new DelimiterSslProtocol("\r\n", "localhost", true, false));
            Assert.IsTrue(client.Connect("127.0.0.1",port));
            
            var data = "test\r\nMessage";
            var reply = client.SendAndGetReply(data);
            Assert.IsNotNull(reply);
            Assert.AreEqual("test",reply.ToString());
        }
    }
}
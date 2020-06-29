using System;
using System.Net;
using System.Threading;
using EasyEncrypt2;
using EasyTcp.Encryption.Protocols;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption
{
    public class EncryptedProtocolsTest
    {
        [Test]
        public void TestEncryption()
        {
            ushort port = TestHelper.GetPort();
            using var encrypter = new EasyEncrypt();

            using var server = new EasyTcpServer().UseServerEncryption(encrypter).Start(port);
            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message);
                message.Client.Send(message);
            };
            using var client = new EasyTcpClient().UseClientEncryption(encrypter);
            
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            Assert.AreEqual("Test", client.SendAndGetReply("Test").ToString());
        }
        
        [Test]
        public void TestEncryptionFail()
        {
            ushort port = TestHelper.GetPort();
            using var encrypter = new EasyEncrypt();

            using var server = new EasyTcpServer().Start(port);
            int isEncrypted = 0;
            server.OnDataReceive += (sender, message) =>
            {
                if (message.ToString() != "Test") Interlocked.Increment(ref isEncrypted);
                Console.WriteLine(message);
                message.Client.Send(message);
            };
            using var client = new EasyTcpClient().UseClientEncryption(encrypter);
            
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            Assert.AreEqual("Test", client.SendAndGetReply("Test").ToString());
            Assert.AreEqual(1,isEncrypted);
        }
    }
}
using System;
using System.Net;
using EasyEncrypt2;
using EasyTcp.Encryption.Protocols;
using EasyTcp3.ClientUtils;
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
            var protocol = new EncryptedPrefixLengthProtocol(encrypter);

            using var server = new EasyTcpServer(protocol).Start(port);
            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message);
                message.Client.Send(message);
            };
            using var client = new EasyTcpClient(protocol);
            
            Assert.IsTrue(client.Connect(IPAddress.Any, port));
            Assert.AreEqual("Test", client.SendAndGetReply("Test").ToString());
        }
    }
}
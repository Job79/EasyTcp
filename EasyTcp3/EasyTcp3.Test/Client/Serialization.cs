/*
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Client
{
    /// <summary>
    /// Tests for the Serialization functions,
    /// Uncomment only if EasyTcp is compiled correctly
    /// </summary>
    public class Serialization
    {
        [Test]
        public void Serialization1()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            MyVeryOwnClass receivedData = null, testData = new MyVeryOwnClass {Id = 10, Name = "MyName"};

            server.OnDataReceive += (sender, message) => receivedData = message.GetObject<MyVeryOwnClass>();

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            client.Send(testData);

            TestHelper.WaitWhileTrue(() => receivedData == null);
            Assert.IsNotNull(receivedData);
            Assert.AreEqual(testData.Id,receivedData.Id);
            Assert.AreEqual(testData.Name,receivedData.Name);
        }
        
        [Test]
        public void Serialization2()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            MyVeryOwnClass receivedData = null, testData = new MyVeryOwnClass {Id = 10, Name = "MyName"};
            
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));
            client.OnDataReceive += (sender, message) => receivedData = message.GetObject<MyVeryOwnClass>();

            server.SendAll(testData);

            TestHelper.WaitWhileTrue(() => receivedData == null);
            Assert.IsNotNull(receivedData);
            Assert.AreEqual(testData.Id,receivedData.Id);
            Assert.AreEqual(testData.Name,receivedData.Name);
        }

        class MyVeryOwnClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
*/
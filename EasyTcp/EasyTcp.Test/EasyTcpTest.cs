using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;
using EasyTcp.Client;
using EasyTcp.Server;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;

namespace EasyTcp.Test
{
    [TestClass]
    public class EasyTcpTest
    {
        [TestMethod]
        public void TestEncryption()
        {
            Encryption Encryption = new Encryption(Aes.Create(),256,"12345","12345678");
            string EncryptedText = Encryption.Encrypt("12345");
            string DecryptedText = Encryption.Decrypt(EncryptedText,Encoding.UTF8);

            Assert.AreEqual("12345",DecryptedText);
        }

        [TestMethod]
        public void TestConnection()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start("0.0.0.0", 1000, 10);
            Server.ClientConnected += (object sender, Socket e) => Console.WriteLine("Client connected");

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect("127.0.0.1", 1000, TimeSpan.FromSeconds(10))) Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestConnectionIPv6()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start(IPAddress.IPv6Any, 1001, 10);
            Server.ClientConnected += (object sender, Socket e) => Console.WriteLine("Client connected");

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect(IPAddress.IPv6Loopback, 1001, TimeSpan.FromSeconds(10))) Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestConnectionIPv6DualSocket()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start(IPAddress.IPv6Any, 1002, 10, true);
            Server.ClientConnected += (object sender, Socket e) => Console.WriteLine("Client connected");

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect(IPAddress.Loopback, 1002, TimeSpan.FromSeconds(10))) Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestServerBan()
        {
            bool ClientRefused = false, Clientconnected = true;
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start("0.0.0.0", 1003, 100);
            Server.BannedIPs.Add("127.0.0.1");
            Server.ClientConnected += (object sender, Socket e) => Clientconnected = false;//ClientConnected should not be called
            Server.ClientRefused += (object sender, RefusedClient e) => ClientRefused = true;//Client should be refused

            EasyTcpClient Client = new EasyTcpClient();
            Client.Connect("127.0.0.1", 1003, TimeSpan.FromSeconds(10));

            Task.Delay(10).Wait();//Little wait
            Assert.IsTrue(!Client.IsConnected&&ClientRefused&&Clientconnected);
        }

        [TestMethod]
        public void TestSendingAndReceiving()
        {
            int ServerReceive = 0, ClientReceive = 0;
            EasyTcpClient Client = new EasyTcpClient() { Encryption = new Encryption(Aes.Create(),256,"12345678","123456789") };
            EasyTcpServer Server = new EasyTcpServer() { Encryption = new Encryption(Aes.Create(), 256, "12345678", "123456789") };
            

            Server.DataReceived += (object sender, Message e) => ServerReceive++;
            Client.DataReceived += (object sender, Message e) => ClientReceive++;

            Server.Start("0.0.0.0", 1004, 101);
            if (!Client.Connect("127.0.0.1", 1004, TimeSpan.FromSeconds(10))) Assert.Fail("Client not connected");
            
            Client.Send(1);
            Server.Broadcast(1);

            Client.SendEncrypted(1);
            Server.BroadcastEncrypted(1);

            Task.Delay(1000).Wait();//Little wait to receive messages

            Console.WriteLine(ServerReceive);
            Console.WriteLine(ClientReceive);

            Assert.IsTrue(ServerReceive.Equals(2),"Server did not receive all messages");
            Assert.IsTrue(ClientReceive.Equals(2), "Client did not receive all messages");

        }
    }
}

/* EasyTcp1.Test
 * 
 * Copyright (c) 2019 henkje
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Net;
using System.Text;
using EasyTcp.Client;
using EasyTcp.Server;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyTcp.Test
{
    [TestClass]
    public class EasyTcpTest
    {
        [TestMethod]
        public void TestEncryption()
        {
            const string input = "input";
            const string password = "Password";
            const string salt = "12345678";

            SymmetricAlgorithm algorithm = TripleDES.Create();
            algorithm.Key = Encryption.CreateKey(algorithm, password, salt);
            string encrypted = new Encryption(algorithm).Encrypt(input);
            string decrypted = new Encryption(algorithm).Decrypt(encrypted);

            Assert.AreEqual(input, decrypted);

            encrypted = new Encryption(algorithm, password, salt).Encrypt(input);
            decrypted = new Encryption(algorithm, password, salt).Decrypt(encrypted);

            Assert.AreEqual(input, decrypted);

            encrypted = new Encryption(algorithm, password, salt).Encrypt(input, Encoding.Unicode);
            decrypted = new Encryption(algorithm, password, salt).Decrypt(encrypted, Encoding.Unicode);

            Assert.AreEqual(input, decrypted);
        }

        [TestMethod]
        public void TestConnection()
        {
            EasyTcpServer server = new EasyTcpServer();
            server.Start("0.0.0.0", 1000, 10);

            EasyTcpClient client = new EasyTcpClient();
            if (!client.Connect("127.0.0.1", 1000, TimeSpan.FromSeconds(1)))
                Assert.Fail("Client not connected.");
        }

        [TestMethod]
        public void TestConnectionIPv6()
        {
            EasyTcpServer server = new EasyTcpServer();
            server.Start(IPAddress.IPv6Any, 1001, 10);

            EasyTcpClient client = new EasyTcpClient();
            if (!client.Connect(IPAddress.IPv6Loopback, 1001, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected.");
        }

        [TestMethod]
        public void TestConnectionIPv6DualSocket()
        {
            EasyTcpServer server = new EasyTcpServer();
            server.Start(IPAddress.IPv6Any, 1002, 10, true);

            EasyTcpClient client = new EasyTcpClient();
            if (!client.Connect(IPAddress.Loopback, 1002, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected.");
        }

        [TestMethod]
        public void TestServerBan()
        {
            bool clientRefused = false, clientConnected = true;
            EasyTcpServer server = new EasyTcpServer();
            server.Start("0.0.0.0", 1003, 100);
            server.BannedIPs.Add("127.0.0.1");
            server.ClientConnected += (object sender, Socket e) => clientConnected = false;//ClientConnected should not be called
            server.ClientRefused += (object sender, RefusedClient e) => clientRefused = true;//Client should be refused

            EasyTcpClient client = new EasyTcpClient();
            client.Connect("127.0.0.1", 1003, TimeSpan.FromSeconds(10));

            Task.Delay(10).Wait();//Little wait
            Assert.IsTrue(!client.IsConnected&&clientRefused&&clientConnected);
        }

        [TestMethod]
        public void TestSendingAndReceiving()
        {
            int serverReceive = 0, clientReceive = 0;
            EasyTcpClient client = new EasyTcpClient() { Encryption = new Encryption(Aes.Create(),256,"12345678","123456789") };
            EasyTcpServer server = new EasyTcpServer() { Encryption = new Encryption(Aes.Create(), 256, "12345678", "123456789") };
           
            server.DataReceived += (object sender, Message e) => serverReceive++;
            client.DataReceived += (object sender, Message e) => clientReceive++;

            server.Start("0.0.0.0", 1004, 101);
            if (!client.Connect("127.0.0.1", 1004, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected");
            
            client.Send(1);
            server.Broadcast(1);

            client.SendEncrypted(1);
            server.BroadcastEncrypted(1);

            Task.Delay(10).Wait();//Little wait to receive messages.

            Console.WriteLine(serverReceive);
            Console.WriteLine(clientReceive);

            Assert.IsTrue(serverReceive.Equals(2),"Server did not receive all messages");
            Assert.IsTrue(clientReceive.Equals(2), "Client did not receive all messages");

        }
    }
}

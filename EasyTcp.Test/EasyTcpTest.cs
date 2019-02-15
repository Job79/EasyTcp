/* EasyTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
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
            const string INPUT = "test";
            const string PASSWORD = "Password";
            const string SALT = "SALT1234567";

            SymmetricAlgorithm Algorithm = TripleDES.Create();
            Algorithm.Key = Encryption.CreateKey(Algorithm, PASSWORD, SALT);
            string Encrypted = new Encryption(Algorithm).Encrypt(INPUT);
            string Decrypted = new Encryption(Algorithm).Decrypt(Encrypted);

            Assert.AreEqual(INPUT, Decrypted);

            Encrypted = new Encryption(Algorithm, PASSWORD, SALT).Encrypt(INPUT);
            Decrypted = new Encryption(Algorithm, PASSWORD, SALT).Decrypt(Encrypted);

            Assert.AreEqual(INPUT, Decrypted);

            Encrypted = new Encryption(Algorithm, PASSWORD, SALT).Encrypt(INPUT, Encoding.Unicode);
            Decrypted = new Encryption(Algorithm, PASSWORD, SALT).Decrypt(Encrypted, Encoding.Unicode);

            Assert.AreEqual(INPUT, Decrypted);
        }

        [TestMethod]
        public void TestConnection()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start("0.0.0.0", 1000, 10);

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect("127.0.0.1", 1000, TimeSpan.FromSeconds(1)))
                Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestConnectionIPv6()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start(IPAddress.IPv6Any, 1001, 10);

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect(IPAddress.IPv6Loopback, 1001, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestConnectionIPv6DualSocket()
        {
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start(IPAddress.IPv6Any, 1002, 10, true);

            EasyTcpClient Client = new EasyTcpClient();
            if (!Client.Connect(IPAddress.Loopback, 1002, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected");
        }

        [TestMethod]
        public void TestServerBan()
        {
            bool ClientRefused = false, ClientConnected = true;
            EasyTcpServer Server = new EasyTcpServer();
            Server.Start("0.0.0.0", 1003, 100);
            Server.BannedIPs.Add("127.0.0.1");
            Server.ClientConnected += (object sender, Socket e) => ClientConnected = false;//ClientConnected should not be called
            Server.ClientRefused += (object sender, RefusedClient e) => ClientRefused = true;//Client should be refused

            EasyTcpClient Client = new EasyTcpClient();
            Client.Connect("127.0.0.1", 1003, TimeSpan.FromSeconds(10));

            Task.Delay(10).Wait();//Little wait
            Assert.IsTrue(!Client.IsConnected&&ClientRefused&&ClientConnected);
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
            if (!Client.Connect("127.0.0.1", 1004, TimeSpan.FromSeconds(10)))
                Assert.Fail("Client not connected");
            
            Client.Send(1);
            Server.Broadcast(1);

            Client.SendEncrypted(1);
            Server.BroadcastEncrypted(1);

            Task.Delay(10).Wait();//Little wait to receive messages

            Console.WriteLine(ServerReceive);
            Console.WriteLine(ClientReceive);

            Assert.IsTrue(ServerReceive.Equals(2),"Server did not receive all messages");
            Assert.IsTrue(ClientReceive.Equals(2), "Client did not receive all messages");

        }
    }
}

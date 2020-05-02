using System.Net;
using System.Text;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Tests for the SendAll functions
    ///
    /// This test does not have using statements,
    /// when added the test fails. TODO
    /// </summary>
    public class SendAllAction
    {
        [Test]
        public void SendAllArray()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            byte[] data = new byte[100];
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionUShort()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            ushort data = 123;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionShort()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            short data = 123;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionUInt()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            uint data = 123;
            server.SendAllAction(0, data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionInt()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            int data = 123;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionULong()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            ulong data = 123;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionLong()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            long data = 123;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionDouble()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            double data = 123.0;
            server.SendAllAction(0,data);
            server.SendAllAction("ECHO",data);
        }

        [Test]
        public void SendAllActionBool()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            server.SendAllAction(0,true);
            server.SendAllAction("ECHO",true);
        }

        [Test]
        public void SendAllActionString()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            string data = "Data";
            server.SendAllAction(0,data);
            server.SendAllAction(0,data, Encoding.UTF32); //Send with different encoding
            server.SendAllAction("ECHO",data);
            server.SendAllAction("ECHO",data, Encoding.UTF32); //Send with different encoding
        }
    }
}
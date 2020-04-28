using System.Net;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Server
{
    public class SendAll
    {
        [Test]
        public void SendAllArray()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            byte[] data = new byte[100];
            server.SendAll(data);
        }

        [Test]
        public void SendUShort()
        {
            ushort port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(port);

            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            ushort data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendShort()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            short data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendUInt()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            uint data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendInt()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            int data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendULong()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            ulong data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendLong()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            long data = 123;
            server.SendAll(data);
        }

        [Test]
        public void SendDouble()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
            
            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            double data = 123.0;
            server.SendAll(data);
        }

        [Test]
        public void SendBool()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            bool data = true;
            server.SendAll(data);
        }

        [Test]
        public void SendString()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            string data = "Data";
            server.SendAll(data);
            server.SendAll(data, Encoding.UTF32); //Send with different encoding
        }
    }
}
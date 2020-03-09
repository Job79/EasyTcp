using System.Net;
using System.Text;
using EasyTcp3.Client;
using EasyTcp3.Server;
using NUnit.Framework;

namespace EasyTcp3.Test.Examples
{
    public class Send
    {
        private ushort port;
        [SetUp]
        public void Setup()
        {
            port = TestHelper.GetPort();
            var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);
        }

        [Test]
        public void SendArray()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            byte[] data = new byte[100];
            client.Send(data);
        }
        
        [Test]
        public void SendUShort()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            ushort data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendShort()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            short data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendUInt()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            uint data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendInt()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            int data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendULong()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            ulong data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendLong()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            long data = 123;
            client.Send(data);
        }
        
        [Test]
        public void SendDouble()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            double data = 123.0;
            client.Send(data);
        }
        
        [Test]
        public void SendBool()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            bool data = true;
            client.Send(data);
        }
        
        [Test]
        public void SendString()
        {
            using var client = new EasyTcpClient();
            client.Connect(IPAddress.Any, port);

            string data = "Data";
            client.Send(data);
            client.Send(data,Encoding.UTF32); //Send with different encoding
        }
    }
}
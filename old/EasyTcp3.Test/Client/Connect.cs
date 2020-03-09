using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using EasyTcp3.Client;
using EasyTcp3.Server;
using EasyTcp3.Server.Helpers;

namespace EasyTcp3.Test.Client
{
    public class Connect
    {
        [Test]
        public void ConnectTest_Valid()
        {
            using var server = new EasyTcpServer();
            using var client = new EasyTcpClient();
            
            server.Start(IPAddress.Any, 2131);
            Assert.IsTrue(client.Connect(IPAddress.Any, 2131), "Client did not connect to server");
        }

        [Test]
        public void ConnectTest_Invalid()
        {
            using var client = new EasyTcpClient();
            Assert.IsFalse(client.Connect(IPAddress.Any, 2132), "Client connected(returned true) to an invalid server");
        }
    }
}
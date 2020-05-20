using System;
using System.Net;
using System.Threading;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Server
{
    /// <summary>
    /// Tests for the OnConnect event
    /// </summary>
    public class OnConnect
    {
        [Test]
        public void OnConnect1()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);

            int connectCount = 0;
            server.OnConnect += (sender, c) =>
            {
                Interlocked.Increment(ref connectCount); //Async lambda, so thread safe increase integer
                Console.WriteLine($"Client {connectCount} connected");
            };

            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect(IPAddress.Any, port));

            TestHelper.WaitWhileTrue(() => connectCount == 0);
            Assert.AreEqual(1, connectCount);
        }
    }
}
using System.Net;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using NUnit.Framework;

namespace EasyTcp3.Test.Server
{
    /// <summary>
    /// Tests for the Start functions
    /// </summary>
    public class Start
    {
        [Test]
        public void Start1()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(new IPEndPoint(IPAddress.Any, port));

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer();
            server2.Start(new IPEndPoint(IPAddress.IPv6Any, port2), true);
        }

        [Test]
        public void Start2()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(IPAddress.Any, port);

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer();
            server2.Start(IPAddress.IPv6Any, port2, true);
        }
        
        [Test]
        public void Start3()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start("127.0.0.1", port);

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer();
            server2.Start("::1", port2, true);
        }

        [Test]
        public void Start4()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer();
            server.Start(port);
        }
    }
}
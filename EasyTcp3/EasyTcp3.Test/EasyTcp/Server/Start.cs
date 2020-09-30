using System.Net;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp.Server
{
    /// <summary>
    /// Tests for all the Start functions
    /// </summary>
    public class Start
    {
        [Test]
        public void Start1()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(new IPEndPoint(IPAddress.Any, port));;

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer().Start(new IPEndPoint(IPAddress.IPv6Any, port2), true);
        }

        [Test]
        public void Start2()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(IPAddress.Any, port);

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer().Start(IPAddress.IPv6Any, port2, true);
        }

        [Test]
        public void Start3()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start("127.0.0.1", port);

            //Start with dualMode socket
            ushort port2 = TestHelper.GetPort();
            using var server2 = new EasyTcpServer().Start("::1", port2, true);
        }

        [Test]
        public void Start4()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using EasyTcp3.Server.ServerUtils.Internal;

namespace EasyTcp3.Server.ServerUtils
{
    /// <summary>
    /// Functions to start a server
    /// </summary>
    public static class StartUtil
    {
        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="endPoint"></param>
        /// <param name="dualMode">specifies if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="backlog">the maximum length of the pending connections queue</param>
        public static EasyTcpServer Start(this EasyTcpServer server, IPEndPoint endPoint, bool dualMode = false,
            int backlog = -1)
        {
            if (server.IsRunning) throw new Exception("Could not start server: server is already running");
            if (endPoint == null) throw new ArgumentException("Could not start server: endPoint is null");
            if (dualMode && endPoint.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Could not start server: use an ipv6 endpoint if using dualMode");

            server.BaseSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (dualMode) server.BaseSocket.DualMode = true;
            server.BaseSocket.Bind(endPoint);
            server.BaseSocket.Listen(backlog);
            server.BaseSocket.BeginAccept(OnConnectUtil.OnClientConnect, server);
            server.IsRunning = true;
            return server;
        }

        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="dualMode">Specifies if the socket is a dual-mode socket (Ipv4 and Ipv6)</param>
        /// <param name="backlog">The maximum length of the pending connections queue</param>
        public static EasyTcpServer Start(this EasyTcpServer server, IPAddress ipAddress, ushort port,
            bool dualMode = false,
            int backlog = -1)
            => Start(server, new IPEndPoint(ipAddress, Math.Max(port, (ushort) 1)), dualMode, backlog);

        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress">ipAddress as string</param>
        /// <param name="port"></param>
        /// <param name="dualMode">Specifies if the socket is a dual-mode socket (Ipv4 and Ipv6)</param>
        /// <param name="backlog">The maximum length of the pending connections queue</param>
        public static EasyTcpServer Start(this EasyTcpServer server, string ipAddress, ushort port,
            bool dualMode = false,
            int backlog = -1)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not start server: ipAddress is not a valid IPv4/IPv6 address");
            return Start(server, new IPEndPoint(address, Math.Max(port, (ushort) 1)), dualMode, backlog);
        }

        /// <summary>
        /// Start listening for new connections on 0.0.0.0
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="backlog">The maximum length of the pending connections queue</param>
        public static EasyTcpServer Start(this EasyTcpServer server, ushort port, int backlog = -1)
            => Start(server, new IPEndPoint(IPAddress.Any, Math.Max(port, (ushort) 1)), false, backlog);
    }
}
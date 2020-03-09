using System;
using System.Net;
using System.Net.Sockets;
using EasyTcp3.Server.Internal;

namespace EasyTcp3.Server
{
    public static class _Start
    {
        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="dualMode">Specifies if the socket is a dual-mode socket (Ipv4 & Ipv6)</param>
        /// <param name="backlog">The maximum length of the pending connections queue</param>
        public static void Start(this EasyTcpServer server, IPEndPoint endPoint, bool dualMode = false, int backlog = 100)
        {
            if (server.IsRunning) throw new Exception("Could not start server: server is already running");
            if (endPoint == null) throw new ArgumentException("Could not start server: endPoint is null");
            if (dualMode && endPoint.AddressFamily == AddressFamily.InterNetworkV6)
                throw new ArgumentException("Could not start server: use an ipv6 endpoint if using dualmode");

            server.BaseSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (dualMode) server.BaseSocket.DualMode = true;
            server.BaseSocket.Bind(endPoint);
            server.BaseSocket.Listen(backlog); 
            server.BaseSocket.BeginAccept(OnClientConnect._OnClientConnect, server);
            server.IsRunning = true;
        }

        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="dualMode">Specifies if the socket is a dual-mode socket (Ipv4 & Ipv6)</param>
        /// <param name="backlog">The maximum length of the pending connections queue</param>
        public static void Start(this EasyTcpServer server, IPAddress ipAddress, ushort port, bool dualMode = false, int backlog = 100)
            => Start(server, new IPEndPoint(ipAddress, Math.Max(port,(ushort)1)), dualMode, backlog);
    }
}
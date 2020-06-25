using System;
using System.Net;
using System.Net.Sockets;

namespace EasyTcp3.Server.ServerUtils
{
    /// <summary>
    /// Class with Start functions
    /// </summary>
    public static class StartUtil
    {
        /// <summary>
        /// Start accepting new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="endPoint">server endpoint</param>
        /// <param name="dualMode">determines if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static EasyTcpServer Start(this EasyTcpServer server, IPEndPoint endPoint, bool dualMode = false,
            Socket socket = null)
        {
            if (server.IsRunning) throw new Exception("Could not start server: server is already running");
            if (endPoint == null) throw new ArgumentException("Could not start server: endPoint is null");
            if (dualMode && endPoint.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Could not start server: use an ipv6 endpoint if using dualMode");

            server.BaseSocket = socket ?? server.Protocol.GetSocket(endPoint.AddressFamily);
            if (dualMode) server.BaseSocket.DualMode = true;
            server.BaseSocket.Bind(endPoint);
            server.Protocol.StartAcceptingClients(server);
            server.IsRunning = true;
            return server;
        }

        /// <summary>
        /// Start accepting new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="dualMode">determines if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static EasyTcpServer Start(this EasyTcpServer server, IPAddress ipAddress, ushort port,
            bool dualMode = false, Socket socket = null)
            => Start(server, new IPEndPoint(ipAddress, Math.Max(port, (ushort) 1)), dualMode, socket);

        /// <summary>
        /// Start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress">ipAddress as string</param>
        /// <param name="port"></param>
        /// <param name="dualMode">determines if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static EasyTcpServer Start(this EasyTcpServer server, string ipAddress, ushort port,
            bool dualMode = false, Socket socket = null)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not start server: ipAddress is not a valid IPv4/IPv6 address");
            return Start(server, new IPEndPoint(address, Math.Max(port, (ushort) 1)), dualMode, socket);
        }

        /// <summary>
        /// Start listening for new connections on 0.0.0.0
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static EasyTcpServer Start(this EasyTcpServer server, ushort port, Socket socket = null)
            => Start(server, new IPEndPoint(IPAddress.Any, Math.Max(port, (ushort) 1)), false, socket);
    }
}
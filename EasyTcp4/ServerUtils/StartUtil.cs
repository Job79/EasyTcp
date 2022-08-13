using System;
using System.Net;
using System.Net.Sockets;

namespace EasyTcp4.ServerUtils
{
    public static class StartUtil
    {
        /// <summary>
        /// Start server and start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="endPoint">server endpoint</param>
        /// <param name="dualMode">determines whether the server is started in dual-mode (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static T Start<T>(this T server, IPEndPoint endPoint, bool dualMode = false,
            Socket socket = null) where T : EasyTcpServer
        {
            if (server == null) throw new Exception("Could not start server: server is null");
            if (server.Protocol == null) throw new Exception("Could not start server: protocol is null");
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
        /// Start server and start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="dualMode">determines if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static T Start<T>(this T server, IPAddress ipAddress, ushort port,
            bool dualMode = false, Socket socket = null) where T : EasyTcpServer
            => Start(server, new IPEndPoint(ipAddress, port), dualMode, socket);

        /// <summary>
        /// Start server and start listening for new connections
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ipAddress">ipAddress as string</param>
        /// <param name="port"></param>
        /// <param name="dualMode">determines if the socket is a dual-mode socket (IPv4 and IPv6)</param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static T Start<T>(this T server, string ipAddress, ushort port,
            bool dualMode = false, Socket socket = null) where T : EasyTcpServer
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not start server: ipAddress is not a valid IPv4/IPv6 address");
            return Start(server, new IPEndPoint(address, port), dualMode, socket);
        }

        /// <summary>
        /// Start server and start listening for new connections on 0.0.0.0
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="socket">baseSocket for EasyTcpServer, new one is create when null</param>
        public static T Start<T>(this T server, ushort port, Socket socket = null) where T : EasyTcpServer
            => Start(server, new IPEndPoint(IPAddress.Any, port), false, socket);
    }
}

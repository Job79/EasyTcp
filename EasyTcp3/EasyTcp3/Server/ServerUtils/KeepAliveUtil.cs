using System;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Server.ServerUtils
{
#if (NETCOREAPP3_0 || NETCOREAPP3_1)
    /// <summary>
    /// Class with all KeepAlive functions for the EasyTcpServer
    /// </summary>
    public static class KeepAliveUtil
    {
        /// <summary>
        /// Enable keep alive
        /// </summary>
        /// <param name="server"></param>
        /// <param name="keepAliveTime">the number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote</param>
        /// <param name="keepAliveInterval">the number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe</param>
        /// <param name="keepAliveRetryCount">the number of TCP keep alive probes that will be sent before the connection is terminated</param>
        /// <exception cref="ArgumentException"></exception>
        public static T EnableServerKeepAlive<T>(this T server, int keepAliveTime = 300, int keepAliveInterval = 30,
            int keepAliveRetryCount = 2) where T : EasyTcpServer
        {
            if (server == null) throw new ArgumentException("Could not enable keepAlive: server is null");

            server.OnConnect += (s, client) =>
                client.BaseSocket.EnableKeepAlive(keepAliveTime, keepAliveInterval, keepAliveRetryCount);
            return server;
        }
    }
#endif
}
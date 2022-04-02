#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0 || NET6_0)
using System;
using System.ComponentModel;
using EasyTcp4.ClientUtils;

namespace EasyTcp4.ServerUtils
{
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
        public static T EnableServerKeepAlive<T>(this T server, int keepAliveTime = 300, int keepAliveInterval = 30, int keepAliveRetryCount = 2)
            where T : EasyTcpServer
        {
            if (server == null) throw new ArgumentException("Could not enable keepAlive: server is null");
            if (server.IsRunning)
                throw new WarningException("Keep alive is only enabled for all upcoming connections, enable keep alive before starting the server");

            server.OnConnect += (s, client) => client.EnableKeepAlive(keepAliveTime, keepAliveInterval, keepAliveRetryCount);
            return server;
        }
    }
}
#endif

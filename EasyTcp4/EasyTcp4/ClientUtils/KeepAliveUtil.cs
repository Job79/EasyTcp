#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0 || NET6_0)
using System;
using System.Net.Sockets;

namespace EasyTcp4.ClientUtils
{
    public static class KeepAliveUtil
    {
        /// <summary>
        /// Enable keep alive
        /// </summary>
        /// <param name="client"></param>
        /// <param name="keepAliveTime">the number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote</param>
        /// <param name="keepAliveInterval">the number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe</param>
        /// <param name="keepAliveRetryCount">the number of TCP keep alive probes that will be sent before the connection is terminated</param>
        /// <exception cref="ArgumentException"></exception>
        public static T EnableKeepAlive<T>(this T client, int keepAliveTime = 300, int keepAliveInterval = 30, int keepAliveRetryCount = 2)
            where T : EasyTcpClient
        {
            if (client == null) throw new ArgumentException("Could not enable keepAlive: client is null");
            if (client.BaseSocket != null) client.BaseSocket.EnableKeepAlive(keepAliveTime, keepAliveInterval, keepAliveRetryCount);
            else client.OnConnect += (s, c) => c.BaseSocket.EnableKeepAlive(keepAliveTime, keepAliveInterval, keepAliveRetryCount);
            return client;
        }

        /// <summary>
        /// Enable keep alive (socket)
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="keepAliveTime">the number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote</param>
        /// <param name="keepAliveInterval">the number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe</param>
        /// <param name="keepAliveRetryCount">the number of TCP keep alive probes that will be sent before the connection is terminated</param>
        internal static void EnableKeepAlive(this Socket socket, int keepAliveTime, int keepAliveInterval, int keepAliveRetryCount)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, keepAliveTime);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, keepAliveInterval);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, keepAliveRetryCount);
        }
    }
}
#endif

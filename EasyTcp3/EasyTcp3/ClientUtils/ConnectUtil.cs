using System;
using System.Net;
using System.Net.Sockets;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to connect to a remote host
    /// </summary>
    public static class ConnectUtil
    {
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host</param>
        /// <param name="port">port of remote host</param>
        /// <param name="timeout">maximum time for connecting to remote host</param>
        /// <param name="socket">socket for EasyTcpClient, new one is create when null</param>
        /// <returns>determines whether the client connected successfully</returns>
        public static bool Connect(this EasyTcpClient client, IPAddress ipAddress, ushort port,
            TimeSpan? timeout = null, Socket socket = null)
        {
            if (client == null) throw new ArgumentException("Could not connect: client is null");
            if (ipAddress == null) throw new ArgumentException("Could not connect: ipAddress is null");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid port");
            if (client.BaseSocket != null) throw new ArgumentException("Could not connect: client is still connected");

            try
            {
                client.BaseSocket = socket ?? new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                var result = client.BaseSocket.BeginConnect(ipAddress, port, null, null);
                result.AsyncWaitHandle.WaitOne(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
                client.BaseSocket.EndConnect(result);

                if (client.BaseSocket.Connected)
                {
                    client.FireOnConnect();
                    client.StartListening();
                    return true;
                }
            }
            catch
            {
                //Ignore exception, dispose (&disconnect) client and return false
            }

            client.Dispose();
            return false;
        }

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host as string</param>
        /// <param name="port">port of remote host</param>
        /// <param name="timeout">maximum time for connecting to remote host</param>
        /// <param name="socket">socket for EasyTcpClient, new one is create when null</param>
        /// <returns>determines whether the client connected successfully</returns>
        /// <exception cref="ArgumentException">ipAddress is not a valid IPv4/IPv6 address</exception>
        public static bool Connect(this EasyTcpClient client, string ipAddress, ushort port, TimeSpan? timeout = null, Socket socket = null)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not connect: ipAddress is not a valid IPv4/IPv6 address");
            return client.Connect(address, port, timeout, socket);
        }
    }
}
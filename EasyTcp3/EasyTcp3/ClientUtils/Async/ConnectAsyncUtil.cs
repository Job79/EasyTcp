using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Functions to async connect to a remote host
    /// </summary>
    public static class ConnectAsyncUtil
    {
        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host</param>
        /// <param name="port">port of remote host</param>
        /// <returns>determines whether the client connected successfully</returns>
        public static async Task<bool> ConnectAsync(this EasyTcpClient client, IPAddress ipAddress, ushort port)
        {
            if (client == null) throw new ArgumentException("Could not connect: client is null");
            if (ipAddress == null) throw new ArgumentException("Could not connect: ipAddress is null");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid port");
            if (client.BaseSocket != null) throw new ArgumentException("Could not connect: client is still connected");

            try
            {
                client.BaseSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await client.BaseSocket.ConnectAsync(ipAddress, port);

                if (client.BaseSocket.Connected)
                {
                    client.Protocol.OnConnect(client);
                    client.FireOnConnect();
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
        /// <returns>determines whether the client connected successfully</returns>
        /// <exception cref="ArgumentException">ipAddress is not a valid IPv4/IPv6 address</exception>
        public static async Task<bool> ConnectAsync(this EasyTcpClient client, string ipAddress, ushort port)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException(
                    "Could not connect to remote host: ipAddress is not a valid IPv4/IPv6 address");
            return await client.ConnectAsync(address, port);
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EasyTcp4.ClientUtils.Async
{
    public static class ConnectAsyncUtil
    {
        /// <summary>
        /// Establish connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endPoint">endPoint of remote host</param>
        /// <param name="socket">base socket for EasyTcpClient, new one is created when null</param>
        /// <returns>determines whether the client connected successfully</returns>
        public static async Task<bool> ConnectAsync(this EasyTcpClient client, EndPoint endPoint, Socket socket = null)
        {
            if (client == null) throw new ArgumentException("Could not connect: client is null");
            if (endPoint == null) throw new ArgumentException("Could not connect: endpoint is null");
            if (client.BaseSocket != null) throw new ArgumentException("Could not connect: client is already connected");

            try
            {
                client.BaseSocket = socket ?? client.Protocol.GetSocket(endPoint.AddressFamily);
                await client.BaseSocket.ConnectAsync(endPoint);

                if (client.BaseSocket.Connected && client.Protocol.OnConnect(client))
                {
                    client.FireOnConnect();
                    return true;
                }
            }
            catch
            {
                // Ignore exception, dispose (&disconnect) client and return false
            }

            client.Dispose(); // Set socket to null
            return false;
        }

        /// <summary>
        /// Establish connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host</param>
        /// <param name="port">port of remote host</param>
        /// <param name="socket">base socket for EasyTcpClient, new one is created when null</param>
        /// <returns>determines whether the client connected successfully</returns>
        public static async Task<bool> ConnectAsync(this EasyTcpClient client, IPAddress ipAddress, ushort port, Socket socket = null)
            => await client.ConnectAsync(new IPEndPoint(ipAddress, port), socket);

        /// <summary>
        /// Establish connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host as string</param>
        /// <param name="port">port of remote host</param>
        /// <param name="socket">base socket for EasyTcpClient, new one is created when null</param>
        /// <returns>determines whether the client connected successfully</returns>
        /// <exception cref="ArgumentException">ipAddress is not a valid IPv4/IPv6 address</exception>
        public static async Task<bool> ConnectAsync(this EasyTcpClient client, string ipAddress, ushort port, Socket socket = null)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not connect to remote host: ipAddress is not a valid IPv4/IPv6 address");
            return await client.ConnectAsync(address, port, socket);
        }
    }
}

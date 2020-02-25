using System;
using System.Net;

namespace EasyTcp3.Client
{
    public static class ConnectHelpers
    {
        private const int DefaultTimeoutInSec = 30;

        public static bool Connect(this EasyTcpClient client, IPAddress address, ushort port)
            => client.Connect(address, port, TimeSpan.FromSeconds(DefaultTimeoutInSec));

        public static bool Connect(this EasyTcpClient client, string address, ushort port)
            => client.Connect(address, port, TimeSpan.FromSeconds(DefaultTimeoutInSec));
        
        public static bool Connect(this EasyTcpClient client, string strAddress, ushort port, TimeSpan timeout)
        {
            if (!IPAddress.TryParse(strAddress, out IPAddress address))
                throw new ArgumentException("Invalid IPv4/IPv6 address");
            return client.Connect(address, port, timeout);
        }
    }
}
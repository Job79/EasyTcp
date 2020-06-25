using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3;
using EasyTcp3.Server;

namespace EasyTcp.Encryption
{
    /// <summary>
    /// Class with functions for enabling ssl
    /// </summary>
    public static class SslUtil
    {
        /// <summary>
        /// Shortcut for enabling ssl
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public static EasyTcpClient UseSsl(this EasyTcpClient client, string serverName, bool acceptInvalidCertificates = false)
        {
            client.Protocol = new PrefixLengthSslProtocol(serverName, acceptInvalidCertificates);
            return client;
        }

        /// <summary>
        /// Shortcut for enabling ssl 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="certificate">server certificate</param>
        public static EasyTcpServer UseSsl(this EasyTcpServer server, X509Certificate certificate)
        {
            server.Protocol = new PrefixLengthSslProtocol(certificate);
            return server;
        }
    }
}
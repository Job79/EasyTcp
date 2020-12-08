using System.Security.Cryptography.X509Certificates;
using EasyTcp3.Encryption.Protocols.Tcp.Ssl;

namespace EasyTcp3.Encryption
{
    public static class SslUtil
    {
        /// <summary>
        /// Shortcut for enabling ssl
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public static T UseSsl<T>(this T client, string serverName, bool acceptInvalidCertificates = false) where T : EasyTcpClient
        {
            client.Protocol = new PrefixLengthSslProtocol(serverName, acceptInvalidCertificates);
            return client;
        }

        /// <summary>
        /// Shortcut for enabling ssl 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="certificate">server certificate</param>
        public static T UseServerSsl<T>(this T server, X509Certificate certificate) where T : EasyTcpServer
        {
            server.Protocol = new PrefixLengthSslProtocol(certificate);
            return server;
        }
    }
}

using System.Security.Cryptography.X509Certificates;
using System.Text;
using EasyTcp4.Encryption.Ssl;
using EasyTcp4.Protocols.Tcp;

namespace EasyTcp4.Encryption
{
    public static class EncryptedProtocolUtil
    {
        /// <summary>
        /// Use the ssl prefix length protocol
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        /// <param name="maxMessageLength">maximimum amount of bytes for one message</param>
        public static T UseSsl<T>(this T client, string serverName, bool acceptInvalidCertificates = false, int maxMessageLength = ushort.MaxValue)
            where T : EasyTcpClient
        {
            client.Protocol = new PrefixLengthSslProtocol(serverName, acceptInvalidCertificates, maxMessageLength);
            return client;
        }

        /// <summary>
        /// Use the ssl prefix length protocol
        /// </summary>
        /// <param name="server"></param>
        /// <param name="certificate">server certificate</param>
        /// <param name="maxMessageLength">maximimum amount of bytes for one message</param>
        public static T UseServerSsl<T>(this T server, X509Certificate certificate, int maxMessageLength = ushort.MaxValue)
            where T : EasyTcpServer
        {
            server.Protocol = new PrefixLengthSslProtocol(certificate, maxMessageLength);
            return server;
        }

        /// <summary>
        /// Use the plain ssl protocol
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        /// <param name="bufferSize">size of the receive buffer, maximum size of a message</param>
        public static T UsePlainTcp<T>(this T client, string serverName, bool acceptInvalidCertificates = false, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpClient
        {
            client.Protocol = new PlainSslProtocol(serverName, acceptInvalidCertificates, bufferSize);
            return client;
        }

        /// <summary>
        /// Use the plain ssl protocol
        /// </summary>
        /// <param name="server"></param>
        /// <param name="certificate">server certificate</param>
        /// <param name="bufferSize">size of the receive buffer, maximum size of a message</param>
        public static T UseServerPlainTcp<T>(this T server, X509Certificate certificate, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpServer
        {
            server.Protocol = new PlainSslProtocol(certificate, bufferSize);
            return server;
        }
    }
}

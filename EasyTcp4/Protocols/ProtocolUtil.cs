using System.Text;
using EasyTcp4.Protocols.Tcp;

namespace EasyTcp4.Protocols
{
    public static class ProtocolUtil
    {
        /// <summary>
        /// Use the delimiter protocol 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="encoding">encoding (default: UTF8)</param>
        public static T UseDelimiter<T>(this T client, string delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null)
            where T : EasyTcpClient
        {
            client.Protocol = new DelimiterProtocol(delimiter, autoAddDelimiter, autoRemoveDelimiter, encoding);
            return client;
        }

        /// <summary>
        /// Use the delimiter protocol 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="delimiter">sequence of bytes that determine the end of a message</param>
        /// <param name="autoAddDelimiter">determines whether the delimiter gets automatically added to the end of a message</param>
        /// <param name="autoRemoveDelimiter">determines whether the delimiter gets automatically removed from a received message</param>
        /// <param name="encoding">encoding (default: UTF8)</param>
        public static T UseServerDelimiter<T>(this T server, string delimiter, bool autoAddDelimiter = true, bool autoRemoveDelimiter = true, Encoding encoding = null)
            where T : EasyTcpServer
        {
            server.Protocol = new DelimiterProtocol(delimiter, autoAddDelimiter, autoRemoveDelimiter, encoding);
            return server;
        }

        /// <summary>
        /// Use the plain tcp protocol
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bufferSize">size of the receive buffer, maximum size of a message</param>
        public static T UsePlainTcp<T>(this T client, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpClient
        {
            client.Protocol = new PlainTcpProtocol(bufferSize);
            return client;
        }

        /// <summary>
        /// Use the plain tcp protocol
        /// </summary>
        /// <param name="server"></param>
        /// <param name="bufferSize">size of the receive buffer, maximum size of a message</param>
        public static T UseServerPlainTcp<T>(this T server, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpServer
        {
            server.Protocol = new PlainTcpProtocol(bufferSize);
            return server;
        }
    }
}

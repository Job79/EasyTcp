using System.Text;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3.Protocols
{
    public static class ProtocolUtil
    {
        /// <summary>
        /// Shortcut for delimiter protocol 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="delimiter"></param>
        /// <param name="autoAddDelimiter"></param>
        /// <param name="autoRemoveDelimiter"></param>
        /// <param name="encoding"></param>
        public static T UseDelimiter<T>(this T client, string delimiter, bool autoAddDelimiter = true,
            bool autoRemoveDelimiter = true, Encoding encoding = null) where T : EasyTcpClient
        {
            client.Protocol = new DelimiterProtocol(delimiter, autoAddDelimiter, autoRemoveDelimiter, encoding);
            return client;
        }

        /// <summary>
        /// Shortcut for delimiter protocol 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="delimiter"></param>
        /// <param name="autoAddDelimiter"></param>
        /// <param name="autoRemoveDelimiter"></param>
        /// <param name="encoding"></param>
        public static T UseServerDelimiter<T>(this T server, string delimiter, bool autoAddDelimiter = true,
            bool autoRemoveDelimiter = true, Encoding encoding = null) where T : EasyTcpServer
        {
            server.Protocol = new DelimiterProtocol(delimiter, autoAddDelimiter, autoRemoveDelimiter, encoding);
            return server;
        }

        /// <summary>
        /// Shortcut for plain tcp protocol 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bufferSize"></param>
        public static T UsePlainTcp<T>(this T client, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpClient
        {
            client.Protocol = new PlainTcpProtocol(bufferSize);
            return client;
        }

        /// <summary>
        /// Shortcut for plain tcp protocol 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="bufferSize"></param>
        public static T UseServerPlainTcp<T>(this T server, int bufferSize = PlainTcpProtocol.DefaultBufferSize)
            where T : EasyTcpServer
        {
            server.Protocol = new PlainTcpProtocol(bufferSize);
            return server;
        }
    }
}

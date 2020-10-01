using System;

namespace EasyTcp3.Logging
{
    /// <summary>
    /// Class with functions to enable logging for 1 server/client 
    /// </summary>
    public static class LoggingUtil
    {
        /// <summary>
        /// Enable logging for client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T UseCustomClientLogging<T>(this T client, Action<LogMessage> logger) where T : EasyTcpClient
        {
            client.OnDataReceive += (sender, message) => logger(new LogMessage(message, LoggingType.DataReceived, sender));
            client.OnDataSend += (sender, message) => logger(new LogMessage(message, LoggingType.DataSend, sender));
            client.OnDisconnect += (sender, c) => logger(new LogMessage(c, LoggingType.ClientDisconnected, sender));
            client.OnConnect += (sender, c) => logger(new LogMessage(c, LoggingType.ClientConnected, sender));
            client.OnError += (sender, exception) => logger(new LogMessage(exception, sender));
            return client;
        }
        
        /// <summary>
        /// Enable logging for server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T UseCustomServerLogging<T>(this T server, Action<LogMessage> logger) where T : EasyTcpServer 
        {
            server.OnDataReceive += (sender, message) => logger(new LogMessage(message, LoggingType.DataReceived, sender));
            server.OnDataSend += (sender, message) => logger(new LogMessage(message, LoggingType.DataSend, sender));
            server.OnConnect += (sender, client) => logger(new LogMessage(client, LoggingType.ClientConnected, sender));
            server.OnDisconnect += (sender, client) => logger(new LogMessage(client, LoggingType.ClientDisconnected, sender));
            server.OnError += (sender, exception) => logger(new LogMessage(exception, sender));
            return server;
        }

        /// <summary>
        /// Enable custom logging for client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T UseClientLogging<T>(this T client, Action<string> logger) where T : EasyTcpClient
            => client.UseCustomClientLogging(message => logger(message.ToString()));

        /// <summary>
        /// Enable custom logging for server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T UseServerLogging<T>(this T server, Action<string> logger) where T : EasyTcpServer 
            => server.UseCustomServerLogging(message => logger(message.ToString()));
    }
}
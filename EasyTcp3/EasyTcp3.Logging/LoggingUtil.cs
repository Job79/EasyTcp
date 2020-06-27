using System;
using EasyTcp3.Server;

namespace EasyTcp3.Logging
{
    //TODO add documentation
    public static class LoggingUtil
    {
        public static T UseClientLogging<T>(this T client, Action<LoggingMessage> logger) where T : EasyTcpClient
        {
            client.OnDataReceive += (sender, message) => logger(new LoggingMessage(message, true));
            client.OnDataSend += (sender, message) => logger(new LoggingMessage(message, false));
            return client;
        }
        
        public static T UseServerLogging<T>(this T server, Action<LoggingMessage> logger) where T : EasyTcpServer 
        {
            server.OnDataReceive += (sender, message) => logger(new LoggingMessage(message, true));
            server.OnDataSend += (sender, message) => logger(new LoggingMessage(message, false));
            return server;
        }

        public static T UseClientLogging<T>(this T client, Action<string> logger) where T : EasyTcpClient
            => client.UseClientLogging(message => logger(LoggingOutputGenerator.GenerateOutput(message)));

        public static T UseServerLogging<T>(this T server, Action<string> logger) where T : EasyTcpServer 
            => server.UseServerLogging(message => logger(LoggingOutputGenerator.GenerateOutput(message)));
        
        public static T UseClientConsoleLogging<T>(this T client) where T : EasyTcpClient
            => client.UseClientLogging((string message) => Console.WriteLine(message));
        
        public static T UseServerConsoleLogging<T>(this T server) where T : EasyTcpServer 
            => server.UseServerLogging((string message) => Console.WriteLine(message));
    }
}
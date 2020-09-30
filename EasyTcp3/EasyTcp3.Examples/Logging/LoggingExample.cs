using System;
using EasyTcp3.ClientUtils;
using EasyTcp3.Logging;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Examples.Logging
{
    /* Example usage of EasyTcp.Logging
     *
     * Logs the following events:
     * DataReceived (MessageString decoded with UTF8)
     * DataSend (MessageString decoded with UTF8)
     * Client connected
     * Client disconnected
     * Internal server/client errors
     *
     * DataSend does print the sent messageString
     * This data does include the framing of the specific protocol (See Examples/Protocols)
     */
    public static class LoggingExample
    {
        private const ushort Port = 1372;
        
        public static void Run()
        {
            // Create server with logging enabled
            // Write log to console
            using var server = new EasyTcpServer().UseServerLogging(Console.WriteLine).Start(Port);
            server.OnDataReceive += (s, message) => message.Client.Send("Hello client test!");
            
            // Create client with logging enabled
            using var client = new EasyTcpClient().UseClientLogging(Console.WriteLine);
            if(!client.Connect("127.0.0.1", Port)) return;
            
             client.Send("Hello server!");
             Console.ReadLine();           
             
            /* Custom logging,
             * only log connect/disconnect events
             */
            using var server2 = new EasyTcpServer().UseCustomServerLogging(logMessage =>
            {
                if(logMessage.Type == LoggingType.ClientConnected) Console.WriteLine(logMessage);
                else if(logMessage.Type == LoggingType.ClientDisconnected) Console.WriteLine(logMessage);
            });
        }
    }
}
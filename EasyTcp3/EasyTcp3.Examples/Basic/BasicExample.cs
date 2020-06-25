using System;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Basic
{
    /* Basic examples for the EasyTcpClient & EasyTcpServer
     */
    public class BasicExample
    {
        private const ushort Port = 5_000;

        public static void Connect()
        {
            // Create new easyTcpClient
            using var client = new EasyTcpClient();
            
            // Connect client
            if(!client.Connect("127.0.0.1", Port)) return;
            else Console.WriteLine("Connected to server!");
            
            // Send hello world to server
            client.Send("Hello world!");
        }
        
        public static void Start()
        {
            // Create new easyTcpServer
            using var server = new EasyTcpServer();

            // Start server
            server.Start(Port);
            
            // Print all receiving data
            server.OnDataReceive += (s, message) => Console.WriteLine(message);
            
            Console.WriteLine("Press enter to shutdown basic server...");
            Console.ReadLine();
        }
    }
}
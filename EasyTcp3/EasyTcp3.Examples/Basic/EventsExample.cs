using System;
using EasyTcp3.ClientUtils;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Examples.Basic
{
    /* Examples that explain all the events for the EasyTcpServer & EasyTcpClient
     */
    public class EventsExample
    {
        public static void Connect()
        {
            using var client = new EasyTcpClient();

            /* OnConnect event,
             * triggered when client connects to the server
             */
            client.OnConnect += (object sender, EasyTcpClient c) => Console.WriteLine("Client: Connected to server");

            /* OnDisconnect event,
             * triggered when client disconnects from server
             */
            client.OnDisconnect += (object sender, EasyTcpClient c) =>
                Console.WriteLine("Client: Disconnected from server");

            /* OnDataReceive event,
             * triggered when client receives any data from the server
             */
            client.OnDataReceive += (object sender, Message message) =>
                Console.WriteLine($"Client: Received data, received: {message}");

            /* OnError event,
             * triggered when an unexpected error occurs withing the clients code or in any event
             * error is thrown when ignored
             */
            client.OnError += (object sender, Exception exception) =>
            {
                Console.WriteLine($"Server: Error occured, message: {exception.Message}");
                Environment.Exit(1);
            };
        }

        public static void StartBasicServer()
        {
            var server = new EasyTcpServer();

            /* OnConnect event,
             * triggered when a new client connects to server
             */
            server.OnConnect += (object sender, EasyTcpClient client) =>
            {
                Console.WriteLine($"Server: Client {server.ConnectedClientsCount} connected");
                client.Send("Welcome to the server!");
            };

            /* OnDisconnect event,
             * triggered when a client disconnects from the server         
             */
            server.OnDisconnect += (object sender, EasyTcpClient client)
                => Console.WriteLine("Server: Client disconnected");

            /* OnDataReceive event,
             * triggered when server receives data from any client
             */
            server.OnDataReceive += OnDataReceive;

            /* OnError event,
             * triggered when an unexpected error occurs withing the servers code or in any event
             * error is thrown when ignored
             */
            server.OnError += (object sender, Exception e) =>
                Console.WriteLine($"Server: Error occured, message: {e.Message}");
                
            Console.WriteLine("Press enter to shutdown basic server...");
            Console.ReadLine();
        }

        private static void OnDataReceive(object sender, Message message)
        {
            Console.WriteLine($"Server: Received data, received: {message}");
            Console.WriteLine("Server: Broadcasting received data");

            EasyTcpClient client = message.Client; // Get client from message
            EasyTcpServer server = (EasyTcpServer) sender; // Convert sender to server or client
            server.SendAll(message);
        }
    }
}
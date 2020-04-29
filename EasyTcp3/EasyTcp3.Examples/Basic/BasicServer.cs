using System;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Basic
{
    /// <summary>
    /// This class contains examples for a basic broadcasting server, (it sends a received package to all connected clients)
    /// this example explains all the events
    /// </summary>
    public static class BasicServer
    {
        private const ushort Port = 5_000;

        public static EasyTcpServer Server = new EasyTcpServer();

        public static void StartBasicServer()
        {
            Server.Start(Port); //Start server on 1 port. (See StartUtil for more options)

            /* Using the OnConnect event.
             * Gets triggered when an new client connects,
             * Has as parameter an EasyTcpClient, events and functions on this client can be used as in BasicClient.cs
             */

            Server.OnConnect += (sender, client) =>
            {
                Console.WriteLine("Server: Client connected");
                Console.WriteLine($"Server: There are now {Server.ConnectedClientsCount+1} clients connected");
            };

            /* Using the OnDisconnect event.
             * Gets triggered when a client is disconnects (After the client is disconnected!).             
             */
            Server.OnDisconnect += (sender, client)
                => Console.WriteLine("Server: Client disconnected");

            /* Using the OnDataReceive event on the function OnDataReceive.
             * This gets triggered when any clients receives data, 
             * remember: Events can also be set for 1 client (See BasicClient.cs)
             */
            Server.OnDataReceive += OnDataReceive;

            /* OnError get triggered when an error occurs in the server code,
             * This includes errors in the events because these are triggered by the server
             */
            Server.OnError += (sender, e) =>
                Console.WriteLine($"Server: Error occured, message: {e.Message}");
        }

        private static void OnDataReceive(object sender, Message message)
        {
            //Print received data, see Message.cs for more functions
            Console.WriteLine($"Server: Received data, received: {message.ToString()}");
            Console.WriteLine("Server: Broadcasting received data");


            //Cast sender to the server, then broadcast message to all connected clients
            ((EasyTcpServer) sender).SendAll(message.Data);
        }
    }
}
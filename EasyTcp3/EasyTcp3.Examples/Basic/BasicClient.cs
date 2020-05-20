using System;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Examples.Basic
{
    /// <summary>
    /// This class contains an examples of a basic client
    /// This example explains all the events
    /// </summary>
    public static class BasicClient
    {
        private const ushort Port = 5_000;

        public static void Connect()
        {
            var client = new EasyTcpClient();

            /* OnConnect event,
             * triggered when client connects to the server
             */
            client.OnConnect += (sender, c) => Console.WriteLine("Client: Connected to server");

            /* OnDisconnect event,
             * triggered when client disconnects from server
             */
            client.OnDisconnect += (sender, c) => { Console.WriteLine("Client: Disconnected from server"); };

            /* OnDataReceive event,
             * triggered when client receives any data from the server
             */
            client.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine($"Client: Received data, received: {message.ToString()}");
            };

            /* OnError get triggered when an error occurs in the server code,
            * This includes errors in the events because these are triggered by the server
            */
            client.OnError += (sender, exception) =>
                Console.WriteLine($"Server: Error occured, message: {exception.Message}");

            /* Connect to server on ip 127.0.0.1 and port 5_000 (Our BasicServer.cs)
             * See ConnectUtil.cs and ConnectAsyncUtil.cs for the connect functions
             */
            bool connected = client.Connect("127.0.0.1", Port);

            if (connected) client.Send("Hello everyone!");
            else Console.WriteLine("Client: Could not connect to server");

            // Send a message and get the reply
            Message reply = client.SendAndGetReply("Hello server!");
        }
    }
}
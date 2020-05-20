using System;
using System.Net;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Examples.Basic
{
    /// <summary>
    /// This class contains a client that sends "Hello" to the echo server and prints the received data ("Hello")
    ///
    /// This is done with 5 lines of code
    /// </summary>
    public static class HelloClient
    {
        private const ushort Port = 5_001;

        public static void Connect()
        {
            var client = new EasyTcpClient();

            //Print received data
            client.OnDataReceive += (sender, message) =>
                Console.WriteLine($"HelloClient: Received \"{message.ToString()}\" from the server");

            //Connect to server, return if failed
            if (!client.Connect(IPAddress.Any, Port)) return;
            client.Send("Hello");
        }
    }
}
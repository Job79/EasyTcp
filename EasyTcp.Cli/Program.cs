using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.Client;
using EasyTcp3.Server;

namespace EasyTcp.cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new EasyTcpServer();

            server.OnClientConnect += (_, socket) => Console.WriteLine("Client connected");
            server.OnClientDisconnect+= (_, socket) => Console.WriteLine("Client disconnected");
            server.OnDataReceive += (_, message) =>
            {
                message.Socket.Send(123);
            };
            
            server.Start(IPAddress.Any, 52521);

            //Task.Delay(-1).Wait();
            Console.ReadLine();
            Console.WriteLine("Shutting down the server...");
            server.Dispose();
            
            while (Console.ReadLine() != "c") Console.WriteLine(server.ConnectedClients.Count());
            Main(null);
            Task.Delay(-1).Wait();
        }
    }
}
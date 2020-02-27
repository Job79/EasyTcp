using System;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.Client;
using EasyTcp3.Helpers;
using EasyTcp3.Server;

namespace EasyTcp.Cli2
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new EasyTcpClient();
            
            client.OnConnect += (_, client) => Console.WriteLine("Client connected to server");
            client.OnDisconnect += (_, client) => Console.WriteLine("Client disconnected from server");
            client.OnDataReceive += (_,message)=>Console.WriteLine(message.ToInt());

            if (!client.Connect(IPAddress.Any, 52521))
            {
                Console.Write("Could not connect");
                return;
            }

            client.Send(1);
            Console.ReadLine();
            //client.Send(new byte[]{ 1, 21, 2, 23 });
        }
    }
}
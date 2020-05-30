using System;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Readme
{
    public class EasyTcp
    {
        public static void Run()
        {
            const ushort port = 100;
            
            using var server = new EasyTcpServer().Start(port);
            server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
            server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
            server.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message}");

            using var client = new EasyTcpClient();
            client.OnConnect += (sender, client) => Console.WriteLine("Client connected!");
            client.OnDisconnect += (sender, client) => Console.WriteLine("Client disconnected!");
            client.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message}");
            
            if(!client.Connect(IPAddress.Loopback, port)) return; 
            client.Send("Hello server");
        } 
    }
}
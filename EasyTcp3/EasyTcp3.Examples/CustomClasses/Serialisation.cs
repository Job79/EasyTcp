using System;
using System.Net;
using System.Text;
using System.Text.Json;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.CustomClasses
{
    /// <summary>
    /// Example usage of serialisation
    /// </summary>
    public static class Serialisation
    {
        const ushort Port = 6524;

        public static void Connect()
        {
            // Create new client with .net core 3 json serialisation
            using var client = new EasyTcpClient
            {
                Serialize = o => JsonSerializer.SerializeToUtf8Bytes(o),
                Deserialize = (b, t) => JsonSerializer.SerializeToUtf8Bytes(b, t)
            };
            if (!client.Connect(IPAddress.Loopback, Port)) return;

            var list = new[] {"testdata", "testdata", "testdata"};
            // Serialize and send array 
            client.Send(list);
        }

        public static void Run()
        {
            // Create new server with json.net serialisation
            var server = new EasyTcpServer
            {
                Serialize = o => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(o)),
                Deserialize = (b, t) => Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(b), t)
            }.Start(Port);

            server.OnDataReceive += (sender, message) =>
            {
                foreach (var line in message.Deserialize<string[]>()) // Deserialize array 
                    Console.WriteLine(line);
            };
        }
    }
}
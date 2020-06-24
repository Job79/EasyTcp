using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Serialisation
{
    /* Serialisation with EasyTcp,
     * EasyTcp provides 2 methods that must be set before using serialisation (serialize(object), deserialize(byte[], Type))
     * See the examples for serialisation with Json(System.Text.Json, Json.Net) and BinaryFormatter
     */
    public static class SerialisationExample
    {
        const ushort Port = 6524;

        public static void Connect()
        {
            // Create new client with .net core 3 json serialisation
            using var client = new EasyTcpClient
            {
                Serialize = o => JsonSerializer.SerializeToUtf8Bytes(o),
                Deserialize = (bytes, type) => JsonSerializer.SerializeToUtf8Bytes(bytes, type)
            };
            if (!client.Connect("127.0.0.1", Port)) return;

            // Serialize and send array 
            client.Send(new[] {"testdata", "testdata", "testdata"});
            
            // Compress message before sending 
            client.Send(new[] {"testdata", "testdata", "testdata"}, true);
        }

        public static void Run()
        {
            // Create new server with json.net serialisation
            var server = new EasyTcpServer
            {
                Serialize = o => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(o)),
                Deserialize = (bytes, type) =>
                    Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type)
            }.Start(Port);

            // Receive and deserialize array
            server.OnDataReceive += (sender, message) =>
            {
                var array = message.Decompress() // Decompress message if compressed 
                    .Deserialize<string[]>();
                Console.WriteLine(string.Join(',', array));
            };
        }

        public static void OtherSerialisationMethods()
        {
            // Serialisation with BinaryFormatter
            using var client = new EasyTcpClient
            {
                Serialize = o =>
                {
                    if (o == null) return null;
                    var bf = new BinaryFormatter();
                    using var ms = new MemoryStream();
                    bf.Serialize(ms, o);
                    return ms.ToArray();
                },
                Deserialize = (b, t) =>
                {
                    using var memStream = new MemoryStream();
                    var binForm = new BinaryFormatter();
                    memStream.Write(b, 0, b.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    return binForm.Deserialize(memStream);
                }
            };
        }
    }
}
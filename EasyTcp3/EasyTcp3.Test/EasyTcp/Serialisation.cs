using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EasyTcp3.Test.EasyTcp
{
    public class Serialisation
    {
        [Test]
        public void TestJsonSerialisation()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);

            using var client = new EasyTcpClient
            {
                Serialize = o => JsonSerializer.SerializeToUtf8Bytes(o),
                Deserialize = (b, t) => JsonSerializer.Deserialize(b, t)
            };
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            var testData = new List<string> {"testdata", "testdata2"};
            var reply = client.SendAndGetReply(testData);
            Assert.AreEqual(testData, reply.Deserialize<List<string>>());
        }

        [Test]
        public void TestJsonNetSerialisation()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);

            using var client = new EasyTcpClient
            {
                Serialize = o => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o)),
                Deserialize = (b, t) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(b), t)
            };
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            var testData = new List<string> {"testdata", "testdata2"};
            var reply = client.SendAndGetReply(testData);
            Assert.AreEqual(testData, reply.Deserialize<List<string>>());
        }

        [Test]
        public void TestStandardSerialisation()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);

            using var client = new EasyTcpClient
            {
                Serialize = o =>
                {
                    if (o == null) return null;
                    BinaryFormatter bf = new BinaryFormatter();
                    using MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, o);
                    return ms.ToArray();
                },
                Deserialize = (b, t) =>
                {
                    using MemoryStream memStream = new MemoryStream();
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(b, 0, b.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    return binForm.Deserialize(memStream);
                }
            };
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            var testData = new List<string> {"testdata", "testdata2"};
            var reply = client.SendAndGetReply(testData);
            Assert.AreEqual(testData, reply.Deserialize<List<string>>());
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using EasyTcp3.ClientUtils;
using Newtonsoft.Json;
using NUnit.Framework;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Test.EasyTcp
{
    public class Serialization
    {
        [Test]
        public void TestJsonSerialization()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);
            using var client = new EasyTcpClient();
            Assert.IsTrue(client.Connect("127.0.0.1", port));

            var testData = new List<string> {"testdata", "testdata2"};
            var reply = client.SendAndGetReply(testData);

            Assert.AreEqual(testData, reply.Deserialize<List<string>>());
        }

        [Test]
        public void TestJsonNetSerialization()
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
        public void TestStandardSerialization()
        {
            ushort port = TestHelper.GetPort();
            using var server = new EasyTcpServer().Start(port);
            server.OnDataReceive += (sender, message) => message.Client.Send(message);

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
            Assert.IsTrue(client.Connect(IPAddress.Loopback, port));

            var testData = new List<string> {"testdata", "testdata2"};
            var reply = client.SendAndGetReply(testData);
            Assert.AreEqual(testData, reply.Deserialize<List<string>>());
        }
    }
}

using System;
using System.Net;
using EasyTcp.Encryption;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Encryption
{
    /// <summary>
    /// Example usage of EasyTcp.Encryption
    /// </summary>
    public static class EncryptionExample
    {
        private const ushort PORT = 3215;
        private const string SALT = "Salt12345678"; // Random salt for each session is recommend if possible

        public static void Start()
        {
            // Create new encryptor instance
            // Default algorithm used is Aes
            // See EasyEncrypt class for more information
            var encrypter = new EasyEncrypt("Key", SALT);

            var server = new EasyTcpServer().Start(PORT);
            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message.Decompress() // Decompress package if compressed
                    .Decrypt(encrypter) // Decrypt message 
                    .ToString()); 
            };
        }

        public static void Connect()
        {
            var client = new EasyTcpClient();
            if (!client.Connect(IPAddress.Loopback, PORT)) return;

            using var encrypter = new EasyEncrypt("Key", SALT);

            // Send encrypted message
            // .Encrypt works on all EasyTcpPackets
            client.Send(EasyTcpPacket.To<Message>("TestMessage").Encrypt(encrypter)
                .Compress()); // Compression also works on all EasyTcpPackets
        }
    }
}
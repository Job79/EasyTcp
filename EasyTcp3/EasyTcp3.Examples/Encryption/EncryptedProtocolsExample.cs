using System;
using EasyEncrypt2;
using EasyTcp.Encryption.Protocols;
using EasyTcp.Encryption.Protocols.Tcp;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Encryption
{
    /// <summary>
    /// Example usage of EasyTcp.Encryption with a custom encrypted protocol.
    /// All data will be automatically encrypted (except SendStream)
    /// See also the examples in the Protocols folder
    /// </summary>
    public class EncryptedProtocolsExample
    {
        private const ushort Port = 3245;
        private static EasyEncrypt encrypter = new EasyEncrypt();

        public static void Connect()
        {
            var protocol = new EncryptedPrefixLengthProtocol(encrypter);

            using var client = new EasyTcpClient(protocol); 
            client.Connect("127.0.0.1", Port);

            client.Send("Data"); // All data is now encrypted before sending
        }

        public static void Run()
        {
            var protocol = new EncryptedPrefixLengthProtocol(encrypter);
            using var server = new EasyTcpServer(protocol).Start(Port);

            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message.ToString()); // Message is automatically decrypted
            };
        }
    }
}
using System;
using System.Security.Cryptography;
using EasyEncrypt2;
using EasyTcp3.ClientUtils;
using EasyTcp3.Encryption;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Encryption
{
    /* EasyTcp.Encryption can be used with EasyEncrypt (https://github.com/Job79/EasyEncrypt)
     * EasyEncrypt supports all SymmetricAlgorithm's
     * See EasyEncrypt.Examples for the usage of EasyEncrypt
     *
     * ! Streams (client.SendStream()) are not automatically encrypted
     */
    public class EncryptedProtocolsExample
    {
        private const ushort Port = 3245;
        private static EasyEncrypt _encrypter;

        public static void Connect()
        {
            /* Create new instance of EasyEncrypt with a password and (hardcoded) salt 
             * Default algorithm is Aes
             */
            _encrypter = new EasyEncrypt("Password", "Salt2135321");
            
            // Create new client with encryption
            var client = new EasyTcpClient().UseClientEncryption(_encrypter); 
            client.Connect("127.0.0.1", Port);

            // All data is now encrypted before sending
            client.Send("Data");
            
            // Encrypter gets disposed with client + protocol 
            client.Dispose();
        }

        public static void Run()
        {
            // Create new instance of EasyEncrypt with a key 
            var encrypter = new EasyEncrypt(Aes.Create(), _encrypter.GetKey());
            
            // Create new server with encryption
            using var server = new EasyTcpServer().UseServerEncryption(encrypter).Start(Port);

            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message.ToString()); // Message is automatically decrypted
            };
        }
    }
}
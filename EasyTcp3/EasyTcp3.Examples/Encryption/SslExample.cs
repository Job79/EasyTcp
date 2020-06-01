using System;
using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Encryption
{
    /// <summary>
    /// Example usage of EasyTcp.Encryption with ssl.
    /// All data will be automatically encrypted with ssl (Except sendStream)
    /// </summary>
    public class SslExample
    {
        private const ushort Port = 3545;

        public static void Connect()
        {
            /* Create new client, then call UseSsl,
             * "localhost" is the servername. This should be the domain name of the server (same as in certificate)
             * Second parameter determines whether the client accepts invalid certificates.
             * This is set to true because our test certificate is invalid
             */
            using var client = new EasyTcpClient().UseSsl("localhost",true); 
            client.Connect("127.0.0.1", Port);

            client.Send("Data"); // All data is now encrypted before sending
        }

        public static void Run()
        {
            /* Load certificate, password of certificate is "password"
             * Test certificate can be found in project
             *
             * Use these commands to create a new certificate with openssl:
             * openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost'
             * openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
             */
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var server = new EasyTcpServer().UseSsl(certificate).Start(Port);

            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message.ToString()); // Message is automatically decrypted
            };
        } 
    }
}
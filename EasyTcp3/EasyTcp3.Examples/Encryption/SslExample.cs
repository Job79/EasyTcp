using System;
using System.Security.Cryptography.X509Certificates;
using EasyTcp.Encryption;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Encryption
{
    /* EasyTcp.Encryption with ssl example
     * All traffic is automatically encrypted
     */
    public class SslExample
    {
        private const ushort Port = 3545;

        public static void Connect()
        {
            /* Create new client with ssl
             * "localhost" is the servername. This should be the domain name of the server (same as in certificate)
             * Second parameter determines whether the client accepts invalid certificates.
             * This is set to true because our test certificate is invalid.
             */
            using var client = new EasyTcpClient().UseSsl("localhost",true); 
            if(!client.Connect("127.0.0.1", Port)) return;
            
            // All data is now encrypted before sending
            client.Send("Data"); 
        }

        public static void Run()
        {
            /* Load certificate, password of test certificate is "password"
             * Test certificate can be found in the project folder
             *
             * Use these commands to create a new certificate with openssl:
             * openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost'
             * openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
             */
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            
            // Create and start server with ssl
            using var server = new EasyTcpServer().UseSsl(certificate).Start(Port);

            server.OnDataReceive += (sender, message) =>
            {
                Console.WriteLine(message); // Message is automatically decrypted
            };
        } 
    }
}
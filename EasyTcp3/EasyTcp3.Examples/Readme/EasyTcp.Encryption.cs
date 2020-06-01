using System;
using System.Net;
using EasyEncrypt2;
using EasyTcp.Encryption;
using EasyTcp.Encryption.Protocols.Tcp;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Readme
{
    public class EasyTcpEncryption
    {
        public static void Run()
        {
            const ushort PORT = 100;

            using var encrypter = new EasyEncrypt("Password","SALT1415312");
            using var server = new EasyTcpServer().Start(PORT);
            server.OnDataReceive += (sender, message) =>
                Console.WriteLine($"Received: {message.Decrypt(encrypter)}");

            using var client = new EasyTcpClient();
            if (!client.Connect(IPAddress.Loopback, PORT)) return;
            client.Send(EasyTcpPacket.To<Message>("Hello Server!").Encrypt(encrypter));
        }
    }
}
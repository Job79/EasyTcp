using System;
using System.Threading.Tasks;
using EasyTcp3.Examples.Actions;
using EasyTcp3.Examples.Basic;
using EasyTcp3.Examples.CustomPacket;
using EasyTcp3.Examples.Encryption;
using EasyTcp3.Examples.Files;
using EasyTcp3.Examples.ReverseShell.Client;
using EasyTcp3.Examples.ReverseShell.Server;
using EasyTcp3.Examples.SpeedTest;

namespace EasyTcp3.Examples
{
    class Program
    {
        static void Main()
        {
            //TestReverseShell();
            
            Console.WriteLine("--    EchoServer/HelloClient    --");
            EchoServer.StartEchoServer();
            HelloClient.Connect();

            Task.Delay(50).Wait(); //Wait 50 milliseconds to let HelloClient finish

            Console.WriteLine("--    BasicServer/BasicClient    --");
            BasicServer.StartBasicServer();
            BasicClient.Connect();

            Task.Delay(50).Wait(); //Wait 50 milliseconds to let BasicClient finish

            Console.WriteLine("--    EasyTcpPacketExample    --");
            EasyTcpPacketExample.Start();
            EasyTcpPacketExample.Connect();

            Task.Delay(50).Wait(); //Wait 50 milliseconds to let EasyTcpPacketExample finish

            Console.WriteLine("--    EncryptionExample    --");
            EncryptionExample.Start();
            EncryptionExample.Connect();

            Task.Delay(50).Wait(); //Wait 50 milliseconds to let EncryptionExample finish

            Console.WriteLine("--    FileServer/FileClient    --");
            FileServer.StartFileServer();
            FileClient.Download("TestFile.txt", "DownloadedTestFile.txt");
            FileClient.Upload("TestFile.txt", "UploadedTestFile.txt");

            Task.Delay(50).Wait(); //Wait 50 milliseconds to let FileClient finish

            Console.WriteLine("--    EchoServer/SpeedTestClient    --");
            SpeedTestClient.RunSpeedTest();

            Console.WriteLine("--    ActionEchoServer/ActionSpeedTestClient    --");
            ActionEchoServer.StartEchoServer();
            ActionSpeedTestClient.RunSpeedTest();

            Task.Delay(-1).Wait();
        }

        private static void TestReverseShell()
        {
            string ip = "127.0.0.1";
            ushort port = 52525;
            var client = new ReverseShellClient();
            var server = new ReverseShellServer();
            var x = client.Start(ip, port);
            server.Start(port);
        }
    }
}
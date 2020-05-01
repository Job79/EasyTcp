using System;
using System.Threading.Tasks;
using EasyTcp3.Examples.Basic;
using EasyTcp3.Examples.Files;
using EasyTcp3.Examples.SpeedTest;

namespace EasyTcp3.Examples
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--    EchoServer/HelloClient    --");
            EchoServer.StartEchoServer();
            HelloClient.Connect();
            
            Task.Delay(50).Wait();//Wait 50 milliseconds to let HelloClient finish

            Console.WriteLine("--    BasicServer/BasicClient    --");
            BasicServer.StartBasicServer();
            BasicClient.Connect();
            
            Task.Delay(50).Wait();//Wait 50 milliseconds to let BasicClient finish

            Console.WriteLine("--    FileServer/FileClient    --");
            FileServer.StartFileServer();
            FileClient.Download("TestFile.txt","DownloadedTestFile.txt");
            FileClient.Upload("TestFile.txt","UploadedTestFile.txt");
            
            Task.Delay(50).Wait();//Wait 50 milliseconds to let FileClient finish
            
            Console.WriteLine("--    EchoServer/SpeedTestClient    --");
            SpeedTestClient.RunSpeedTest();
            
            Task.Delay(-1).Wait();
        }
    }
}
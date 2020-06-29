using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.SpeedTest
{
    /// <summary>
    /// This class contains a basic speedtest of the SendActionAndGetReply method,
    /// this includes the Send and Receive functions + the actions performance 
    ///
    /// It uses the action echo server as test server
    /// </summary> 
    public static class ActionSpeedTestClient
    {
        const int Port = 5_001;
        const int MessageCount = 1000_000;
        const string Message = "Message";

        public static void RunSpeedTest()
        {
            using var server = new EasyTcpActionServer().Start(Port);
            
            var client = new EasyTcpClient();
            if (!client.Connect(IPAddress.Loopback, Port)) return;

            byte[] message = Encoding.UTF8.GetBytes(Message);
            using var resetEvent = new ManualResetEventSlim();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int counter = 0;
            client.OnDataReceive += (o, message) =>
            {
                if(Interlocked.Increment(ref counter) == MessageCount) resetEvent.Set();
            };

            for (int x = 0; x < MessageCount; x++) client.SendAction("ECHO", message);

            resetEvent.Wait();
            Console.WriteLine($"Send {counter} action messages");
            Console.WriteLine($"ElapsedMilliseconds SpeedTest: {sw.ElapsedMilliseconds}");
            Console.WriteLine($"Average SpeedTest: {sw.ElapsedMilliseconds / (double) MessageCount}");
            Console.WriteLine($"Messages/Second: {MessageCount / sw.Elapsed.TotalSeconds}");
        }
    }
}
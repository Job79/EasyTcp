using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Protocols;
using EasyTcp3.Protocols.Tcp;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.SpeedTest
{
    public class MultiThreadedActionSpeedTest
    {
        const int ClientsCount = 20_000; // Max: ushort / 2 because of ip restrictions
        const int MessageCount = 5_000_000;
        private static readonly string MessageDataString = new string('H', 100);
        const int Port = 50013;
        private const int ThreadAmount = 8;
        private static readonly IEasyTcpProtocol ServerProtocol = new PrefixLengthProtocol();
        private static readonly IEasyTcpProtocol ClientProtocol = new PrefixLengthProtocol();

        public static void Run()
        {
            using var server = new EasyTcpActionServer(ServerProtocol).Start(Port);
            
            byte[] messageData = Encoding.UTF8.GetBytes(MessageDataString);
            var clientList = new ConcurrentQueue<EasyTcpClient>();
            int counter = 0;
            using var resetEvent = new ManualResetEventSlim();
            Stopwatch st = Stopwatch.StartNew();
            
            Parallel.For(0, ClientsCount, new ParallelOptions {MaxDegreeOfParallelism = ThreadAmount}, i =>
            {
                var client = new EasyTcpClient((IEasyTcpProtocol) ClientProtocol.Clone());
                client.OnDataReceive += (o, message) =>
                {
                    if(Interlocked.Increment(ref counter) == MessageCount) resetEvent.Set();
                };
                if (client.Connect(IPAddress.Any, Port)) clientList.Enqueue(client);
            });
            Console.WriteLine($"Connected with {clientList.Count} clients");
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per client: {st.ElapsedMilliseconds / (double) clientList.Count}");
            Console.WriteLine($"Connections/Second: {clientList.Count / st.Elapsed.TotalSeconds}");

            Console.WriteLine($"\n\nSending {MessageCount} messages");
            st.Restart();
            Parallel.For(0, MessageCount, new ParallelOptions {MaxDegreeOfParallelism = ThreadAmount}, i =>
            {
                clientList.TryDequeue(out EasyTcpClient client);
                client.SendAction("ECHO", messageData);
                clientList.Enqueue(client);
            });

            resetEvent.Wait();
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per message: {st.ElapsedMilliseconds / (double) MessageCount}");
            Console.WriteLine($"Messages/Second: {MessageCount / st.Elapsed.TotalSeconds}");

            foreach (var client in clientList) client.Dispose();
            Console.ReadLine();
        }
    }
}
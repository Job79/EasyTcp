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
    public static class MultiThreadedSpeedTest
    {
        const int ClientsCount = 10_000; // Max: ushort / 2 because of ip restrictions
        const int Messages = 2_000_000;
        private static readonly string MessageDataString = new string('H', 100);
        const int Port = 50013;
        private const int ThreadAmount = 8;
        private static readonly IEasyTcpProtocol ServerProtocol = new PrefixLengthProtocol();
        private static readonly IEasyTcpProtocol ClientProtocol = new PrefixLengthProtocol();

        public static void Run()
        {
            using var server = new EasyTcpActionServer(ServerProtocol).Start(Port);
            byte[] messageData = Encoding.UTF8.GetBytes(MessageDataString);

            Stopwatch st = Stopwatch.StartNew();
            var clientList = new ConcurrentBag<EasyTcpClient>();
            Parallel.For(0, ClientsCount, new ParallelOptions {MaxDegreeOfParallelism = ThreadAmount}, i =>
            {
                var client = new EasyTcpClient((IEasyTcpProtocol) ClientProtocol.Clone());
                if (client.Connect(IPAddress.Any, Port)) clientList.Add(client);
            });
            Console.WriteLine($"Connected with {clientList.Count} clients");
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per client: {st.ElapsedMilliseconds / (double) clientList.Count}");
            Console.WriteLine($"Connections/Second: {clientList.Count / st.Elapsed.TotalSeconds}");
            st.Restart();

            Console.WriteLine($"\n\nSending {Messages} messages");
            Parallel.For(0, Messages, new ParallelOptions {MaxDegreeOfParallelism = ThreadAmount}, i =>
            {
                clientList.TryTake(out EasyTcpClient client);
                var message = client.SendActionAndGetReply("ECHO", messageData);
                clientList.Add(client);
            });
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per message: {st.ElapsedMilliseconds / (double) Messages}");
            Console.WriteLine($"Messages/Second: {Messages / st.Elapsed.TotalSeconds}");

            foreach (var client in clientList) client.Dispose();
            Console.ReadLine();
        }
    }
}
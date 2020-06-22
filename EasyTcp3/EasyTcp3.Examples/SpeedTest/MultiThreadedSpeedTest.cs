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
        const int clientsCount = 10_000; // Max: ushort / 2 because of ip restrictions
        const int messages = 2_000_000;
        private static string messageDataString = new string('H', 100);
        const int port = 50013;
        private const int threadAmount = 8;

        public static void Run()
        {
            var serverProtocol = new PrefixLengthProtocol();
            var clientProtocol = new PrefixLengthProtocol();
            using var server = new EasyTcpActionServer(serverProtocol).Start(port);
            byte[] messageData = Encoding.UTF8.GetBytes(messageDataString);

            Stopwatch st = Stopwatch.StartNew();
            var clientList = new ConcurrentBag<EasyTcpClient>();
            Parallel.For(0, clientsCount, new ParallelOptions() {MaxDegreeOfParallelism = threadAmount}, i =>
            {
                var client = new EasyTcpClient((IEasyTcpProtocol) clientProtocol.Clone());
                if (client.Connect(IPAddress.Any, port)) clientList.Add(client);
            });
            Console.WriteLine($"Connecting with {clientList.Count} clients");
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per client: {st.ElapsedMilliseconds / (double) clientList.Count}");
            st.Restart();

            Parallel.For(0, messages, new ParallelOptions() {MaxDegreeOfParallelism = threadAmount}, (i) =>
            {
                clientList.TryTake(out EasyTcpClient client);
                var message = client.SendActionAndGetReply("ECHO", messageData);
                clientList.Add(client);
            });
            Console.WriteLine($"Sending {messages} messages");
            Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average milliseconds per message: {st.ElapsedMilliseconds / (double) messages}");

            foreach (var client in clientList) client.Dispose();
            Console.ReadLine();
        }
    }
}
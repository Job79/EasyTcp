using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.Protocols.Tcp;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.SpeedTest
{
    public class ThroughputTest
    {
        const int ClientsCount = 100; // Max: ushort / 2 because of ip restrictions
        private static readonly string MessageDataString = new string('H', 100);
        const int Port = 1111;
        private const int ThreadAmount = 16;

        public static void Run()
        {
            using var server = new EasyTcpServer().Start(Port);
            server.OnDataReceive += (o, message) => message.Client.Send(message);

            byte[] messageData = Encoding.UTF8.GetBytes(MessageDataString);
            var clientList = new ConcurrentQueue<EasyTcpClient>();
            int counter = 0;
            Stopwatch st = Stopwatch.StartNew();
            
            Parallel.For(0, ClientsCount, new ParallelOptions {MaxDegreeOfParallelism = ThreadAmount}, i =>
            {
                var client = new EasyTcpClient();
                client.OnConnect += (o, client) => client.Send(messageData);
                client.OnDataReceive += (o,message) =>
                {
                    Interlocked.Increment(ref counter);
                    message.Client.Send(message);
                };
                var socket = client.Protocol.GetSocket(AddressFamily.InterNetwork);
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                if (client.Connect(IPAddress.Any, Port, socket: socket)) clientList.Enqueue(client);
            });

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Total: {st.ElapsedMilliseconds}ms, {counter} messages");
                Console.WriteLine($"Average milliseconds per message: {st.ElapsedMilliseconds / (double) counter}");
                Console.WriteLine($"Messages/Second: {counter / st.Elapsed.TotalSeconds}"); 
                Task.Delay(50).Wait();
            }
        }
    }
}
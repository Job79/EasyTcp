using System;
using System.Net;
using System.Threading.Tasks;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Basic
{
    /* Examples that explain the usage of the basic Server & ClientUtils
     */
    public static class UtilsExamples 
    {
        const ushort Port = 1242;
        public static async Task Connect()
        {
            using var client = new EasyTcpClient();
            var socket = client.BaseSocket; // Get baseSocket, null when client is disposed/disconnected
            
            // Connect to remote host with IP and Port
            if(!client.Connect("127.0.0.1", Port)) return;
            if(!client.Connect(IPAddress.Loopback, Port)) return;
            
            // Connect to remote host with IP, Port and timeout (Default: 5 seconds)
            if(!client.Connect("127.0.0.1", Port, TimeSpan.FromSeconds(10))) return;
            
            // Async connect to remote host with IP and Port
            if(!await client.ConnectAsync("127.0.0.1", Port)) return;
            
            // Async send data to server
            client.Send("Hello server!");
            client.Send(1);
            client.Send(1.000);
            client.Send(true);
            client.Send("Hello server!", true); // Async send compressed data
            
            /* Send data to server and get reply
             * OnDataReceive is not triggered when receiving a reply
             * 
             * ! SendAndGetReply does not work in OnDataReceive
             * ! first call client.Protocol.EnsureDataReceiverIsRunning(client) when using in OnDataReceive
             */
            var reply = client.SendAndGetReply("data");
            
            // Send data to server and get reply with a specified timeout (Default: 5 seconds)
            // SendAndGetReply returns null when no message is received within given time frame
            var replyWithTimeout = client.SendAndGetReply("data", TimeSpan.FromSeconds(5));
            
            // Send data to server and get reply async
            var reply2 = await client.SendAndGetReplyAsync("data");

            /* Get next received data as variable
             * OnDataReceive is not triggered
             */
            var receivedData = await client.ReceiveAsync();
            
            // Use timeout (Default: infinite)
            // Returns null when no message is received within given time frame
            var receivedDataWithTimeout = await client.ReceiveAsync(TimeSpan.FromSeconds(5));
            
            // Determines whether client is still connected
            bool isConnected = client.IsConnected();
            bool isConnectedWithPoll = client.IsConnected(true); // Use poll for a more accurate (but slower) result
        }
        
        public static async Task Start()
        {
            using var server = new EasyTcpServer();
            var socket = server.BaseSocket; // Get baseSocket, null when server is disposed
            
            // Start server on given port (default address: "0.0.0.0")
            server.Start(Port);
            
            // Start server on given port and address
            server.Start("0.0.0.0", Port);
            server.Start(IPAddress.Any, Port);
            
            // Send message to all connected clients
            server.SendAll("Hello clients!");

            // Get copy of list with connected clients
            var connectedClientsList = server.GetConnectedClients();
            var connectedSocketsList = server.GetConnectedSockets();
            
            // Get amount of connected clients 
            var connectedClientsCount = server.ConnectedClientsCount;

            server.OnDataReceive += (sender, message) =>
            {
                // Get client
                var client = message.Client;

                // Get endpoint or ip address of client
                var endPoint = client.GetEndPoint();
                var ip = client.GetIp();
                
                // Use all the other clientUtil on client
                client.Send("Hello from server!");

                // Determines whether message is compressed
                var isCompressed = message.IsCompressed();
                
                /* Decompress message,
                 * ignored when message is not compressed
                 */
                var data = message.Decompress(); 
            };
        }
    }
}

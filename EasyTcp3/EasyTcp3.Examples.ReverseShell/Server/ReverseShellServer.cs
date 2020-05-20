using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.ReverseShell.Server
{
    /// <summary>
    /// Example reverse shell with EasyTcp3 & EasyTcp.Actions
    /// </summary>
    public class ReverseShellServer
    {
        public void Start(ushort port)
        {
            EasyTcpClient selectedClient = null; // Current selected client, commands get send to this client
            using var server = new EasyTcpServer().Start(port);
            server.OnError += (s, exception) => Console.WriteLine($"Server error: {exception.Message}");
            server.OnDataReceive += (s, message) =>
            {
                if (selectedClient == message.Client) Console.WriteLine(message.Decompress().ToString());
            };

            while (true)
            {
                string command = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(command)) continue;

                var args = command.Split(' ');
                command = args[0];

                ExecuteCommand(command, args.Skip(1).ToArray(), server, ref selectedClient);
            }
        }

        /// <summary>
        /// Execute user command
        /// </summary>
        /// <param name="command">name of action (On client)</param>
        /// <param name="args">command arguments</param>
        /// <param name="server">current server</param>
        /// <param name="selectedClient">current selected client</param>
        private void ExecuteCommand(string command, string[] args, EasyTcpServer server,
            ref EasyTcpClient selectedClient)
        {
            if (command == "s" || command == "select")
                selectedClient = SelectClient(new List<EasyTcpClient>(server.GetConnectedClients()));
            else
                selectedClient?.SendAction(command, args.Any() ? Encoding.UTF8.GetBytes(string.Join(':', args)) : null,
                    true);
        }

        /// <summary>
        /// Let user pick 1 client and return selected client
        /// </summary>
        /// <param name="clients">list of clients</param>
        /// <returns>selected client or null</returns>
        private EasyTcpClient SelectClient(List<EasyTcpClient> clients)
        {
            if (!clients.Any())
            {
                Console.WriteLine("There are no connected clients");
                return null;
            }

            Console.WriteLine($"Connected clients[{clients.Count}]:");
            for (int i = 0; i < clients.Count; i++)
                Console.WriteLine($"{i + 1}\t{clients[i].GetIp()}\t{clients[i].GetHashCode()}");

            if (!int.TryParse(Console.ReadLine(), out var client) || (client -= 1) < 0 || client >= clients.Count)
                return SelectClient(clients);
            return clients[client];
        }
    }
}
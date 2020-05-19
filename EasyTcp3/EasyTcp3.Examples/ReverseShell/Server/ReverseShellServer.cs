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
    public class ReverseShellServer
    {
        public void Start(ushort port)
        {
            EasyTcpClient selectedClient = null;
            using var server = new EasyTcpServer().Start(port);
            server.OnError += (s, exception) => Console.WriteLine($"Server error: {exception.Message}");
            server.OnDataReceive += (s, message) =>
            {
                if(selectedClient != message.Client) return;
                Console.WriteLine(message.Decompress().ToString());
            };

            while (true)
            {
                Console.Write(">");
                string command = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(command)) continue;
                
                var args = command.Split(' ');
                command = args[0];

                ExecuteCommand(command,args.Skip(1).ToArray(), server, ref selectedClient);
            }
        }

        private void ExecuteCommand(string command, string[] args, EasyTcpServer server,
            ref EasyTcpClient selectedClient)
        {
            if (command == "s" || command == "select")
                selectedClient = SelectClient(new List<EasyTcpClient>(server.GetConnectedClients()));
            else selectedClient?.SendAction(command, args.Any()?Encoding.UTF8.GetBytes(string.Join(':', args)):null, compression: true);
        }

        private EasyTcpClient SelectClient(List<EasyTcpClient> clients)
        {
            if (!clients.Any())
            {
                Console.WriteLine("There are no connected clients");
                return null;
            }

            Console.WriteLine($"Connected clients[{clients.Count}]:");
            for (int i = 0; i < clients.Count; i++) Console.WriteLine($"{i+1}\t{clients[i].GetIp()}\t{clients[i].GetHashCode()}");
            Console.Write(">");

            if (!int.TryParse(Console.ReadLine(), out var client) || (client -= 1) < 0 || client >= clients.Count)
                return SelectClient(clients);
            return clients[client];
        }
    }
}
using System;
using System.Text;
using EasyTcp3.Client;

namespace EasyTcp3.Server
{
    public static class _SendAll
    {
        public static void SendAll(this EasyTcpServer server, byte[] data)
        {
            foreach (var client in server.GetConnectedClients()) client.Send(data);
        }
        
        public static void SendAll(this EasyTcpServer server, ushort data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, short data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, uint data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, int data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, ulong data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, long data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, double data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, bool data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null)
            => server.SendAll((encoding??Encoding.UTF8).GetBytes(data));
    }
}
using System;
using System.Net.Sockets;

namespace EasyTcp3.Server.Helpers
{
    public static class SendAllHelper
    {
        public static void SendAll(this EasyTcpServer server, byte[] data)
        {
            if (data == null) throw new ArgumentException("Could not send data: Data is null.");
            if (!server.IsRunning) throw new ArgumentException("Could not send data: Server is not running.");

            byte[] message = new byte[2 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);

            using SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            foreach (var client in server.ConnectedClients) client.SendAsync(e);
        }
        
        public static void SendAll(this EasyTcpServer server, ushort data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, short data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, uint data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, int data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, ulong data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, long data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, double data) => server.SendAll(BitConverter.GetBytes(data));
        public static void SendAll(this EasyTcpServer server, bool data) => server.SendAll(BitConverter.GetBytes(data));
    }
}
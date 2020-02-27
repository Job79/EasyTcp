using System;
using System.Net.Sockets;

namespace EasyTcp3.Client
{
    public static class SendHelper
    {
        public static void Send(this EasyTcpClient client, byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Could not send data: Data is empty.");
            if (!client.IsConnected(true)) throw new Exception("Could not send data: Socket not connected.");

            var message = new byte[2 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length),
                0, message, 0, 2); //Write length of data to message.
            Buffer.BlockCopy(data, 0, message, 2, data.Length); //Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            client.BaseSocket.SendAsync(e);
        }

        public static void Send(this EasyTcpClient client, ushort data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, short data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, uint data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, int data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, ulong data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, long data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, double data) => client.Send(BitConverter.GetBytes(data));
        public static void Send(this EasyTcpClient client, bool data) => client.Send(BitConverter.GetBytes(data));
    }
}
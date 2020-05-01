using System;
using System.Net.Sockets;

namespace EasyTcp3.Actions.ActionUtils
{
    public static class SendActionUtil
    {
        public static void SendAction(this EasyTcpClient client, int action, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            var message = new byte[2 + 4 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length + 4),
                0, message, 0, 2); // Write length of data to message.
            Buffer.BlockCopy(BitConverter.GetBytes(action),
                0, message, 2, 4); // Write action operationCode to message.
            Buffer.BlockCopy(data, 0, message, 6, data.Length); // Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message);
            client.BaseSocket.SendAsync(e);
        }
    }
}
using System;
using System.Net.Sockets;

namespace EasyTcp3.Client.Internal
{
    internal static class _OnReceive
    {
        /// <summary>
        /// Start listening for incoming data for a specific client
        /// </summary>
        /// <param name="client"></param>
        internal static void StartListening(EasyTcpClient client)
            => client.BaseSocket.BeginReceive(client.Buffer = new byte[2], 0, client.Buffer.Length, SocketFlags.None,
                OnReceive, client);


        /// <summary>
        /// Function that gets triggered when data is received
        /// </summary>
        /// <param name="ar">EasyTcpClient as IAsyncResult</param>
        private static void OnReceive(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client == null) return; 

            try
            {
                ushort dataLength = 2;
                if (client.ReceivingData = !client.ReceivingData) //If receiving length
                {
                    if ((dataLength = BitConverter.ToUInt16(client.Buffer, 0)) == 0)
                    {
                        HandleDisconnect(client);
                        return;
                    }
                }
                else client.FireOnDataReceive(new Message(client.Buffer, client));

                client.BaseSocket.BeginReceive(client.Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
                    OnReceive, client);
            }
            catch (SocketException)
            {
                HandleDisconnect(client);
            }
            catch (Exception ex)
            {
                client.FireOnError(ex);
            }
        }

        internal static void HandleDisconnect(EasyTcpClient client)
        {
            client.FireOnDisconnect();
            client.Dispose();
        }
    }
}
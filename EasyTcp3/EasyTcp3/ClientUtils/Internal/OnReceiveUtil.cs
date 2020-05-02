using System;
using System.Net.Sockets;

namespace EasyTcp3.ClientUtils.Internal
{
    /// <summary>
    /// Internal functions to receive messages from a remote host
    /// 
    /// EasyTcp Protocol: 
    /// 1. Begin receiving data (async event)
    /// 2. Receive length of the data, allocate buffer of given length (length is an ushort, 2 bytes)
    /// 3. Receive data with the specified length
    /// 4. Go back to step 1
    /// </summary>
    internal static class OnReceiveUtil
    {
        /// <summary>
        /// Start listening for incoming data for a specific client
        /// </summary>
        /// <param name="client"></param>
        internal static void StartListening(this EasyTcpClient client)
            => client.BaseSocket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, OnReceive,
                client);

        /// <summary>
        /// Function that gets triggered when data is received
        /// </summary>
        /// <param name="ar"></param>
        private static void OnReceive(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client?.BaseSocket == null) return;

            try
            {
                ushort dataLength;
                if ((dataLength = HandleData(client)) == 0)
                {
                    client.HandleDisconnect();
                    return;
                }

                client.BaseSocket
                    .EndReceive(ar); // Return value isn't used because our protocol uses all available bytes
                client.BaseSocket.BeginReceive(client.Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
                    OnReceive, client);
            }
            catch (SocketException)
            {
                client.HandleDisconnect();
            }
            catch (Exception ex)
            {
                client.FireOnError(ex);
            }
        }

        /// <summary>
        /// Handle received data and then return length of next incoming data
        /// </summary>
        /// <param name="client"></param>
        /// <returns>length of next incoming data</returns>
        private static ushort HandleData(EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (client.ReceivingData) client.DataReceiveHandler(new Message(client.Buffer, client));
            else dataLength = BitConverter.ToUInt16(client.Buffer, 0);

            client.ReceivingData ^= true;
            return dataLength;
        }

        /// <summary>
        /// Handle disconnect for a specific client
        /// </summary>
        /// <param name="client"></param>
        internal static void HandleDisconnect(this EasyTcpClient client)
        {
            client.FireOnDisconnect();
            client.Dispose();
        }
    }
}
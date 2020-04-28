using System;
using System.Net.Sockets;

namespace EasyTcp3.ClientUtils.Internal
{
    /// <summary>
    /// Protocol: 
    /// 1. Begin receiving data (Event)
    /// 2. Receive length of the data, allocate buffer of given length (length is an ushort, 2 bytes)
    /// 3. Receive data with the specified length
    /// 4. Go back to step 1
    /// </summary>
    public static class OnReceiveUtil
    {
        /// <summary>
        /// Start listening for incoming data for a specific client
        /// </summary>
        /// <param name="client"></param>
        internal static void StartListening(EasyTcpClient client)
            => client.BaseSocket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, OnReceive, client);
        
        /// <summary>
        /// Function that gets triggered when data is received
        /// </summary>
        /// <param name="ar"></param>
        private static void OnReceive(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client == null) return; 

            try
            {
                ushort dataLength = 2;
                client.ReceivingData ^= true; 
                if (client.ReceivingData) //If receiving length
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

        /// <summary>
        /// Handle disconnect for a specific client
        /// </summary>
        /// <param name="client"></param>
        internal static void HandleDisconnect(EasyTcpClient client)
        {
            client.FireOnDisconnect();
            client.Dispose();
        }
    }
}
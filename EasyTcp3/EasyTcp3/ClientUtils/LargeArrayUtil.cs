using System;
using System.IO;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with the SendLargeArray/ReceiveLargeArray functions 
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// Send array to the remote host
        /// Host can only receive an array when not listening for incoming messages
        /// </summary>
        /// <param name="client"></param>
        /// <param name="array"></param>
        /// <param name="sendLengthPrefix"></param>
        public static void SendLargeArray(this EasyTcpClient client, byte[] array, bool sendLengthPrefix = true)
        {
            using var networkStream = client.Protocol.GetStream(client);
            if(sendLengthPrefix) networkStream.Write(BitConverter.GetBytes(array.Length));
            networkStream.Write(array);
        }

        /// <summary>
        /// Receive array from remote host
        /// Use this method only when not listening for incoming messages (In the OnReceive event)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="count"></param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static byte[] ReceiveLargeArray(this Message message, int count = 0, int bufferSize = 1024)
        {
            using var networkStream = message.Client.Protocol.GetStream(message.Client);

            if (count == 0)
            {
                // Get length from stream
                var length = new byte[4];
                networkStream.Read(length, 0, length.Length);
                count = BitConverter.ToInt32(length, 0);
            }

            var receivedArray = new byte[count];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < count &&
                   (read = networkStream.Read(receivedArray, totalReceivedBytes,
                       Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
                totalReceivedBytes += read;
            return receivedArray;
        }
    }
}
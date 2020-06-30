using System;
using System.IO;
using System.Threading.Tasks;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Class with the SendLargeArrayAsync/ReceiveLargeArrayAsync functions 
    /// </summary>
    public static class ArrayAsyncUtil
    {
        /// <summary>
        /// Send array to the remote host
        /// Host can only receive an array when not listening for incoming messages
        /// </summary>
        /// <param name="client"></param>
        /// <param name="array"></param>
        /// <param name="sendLengthPrefix"></param>
        public static async Task SendLargeArrayAsync(this EasyTcpClient client, byte[] array, bool sendLengthPrefix = true)
        {
            await using var networkStream = client.Protocol.GetStream(client); 
            if(sendLengthPrefix) await networkStream.WriteAsync(BitConverter.GetBytes(array.Length));
            await networkStream.WriteAsync(array);
        }

        /// <summary>
        /// Receive array from remote host
        /// Use this method only when not listening for incoming messages (In the OnReceive event)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="count"></param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static async Task<byte[]> ReceiveLargeArrayAsync(this Message message, int count = 0, int bufferSize = 1024)
        {
            await using var networkStream = message.Client.Protocol.GetStream(message.Client);

            // Get length from stream
            if (count == 0)
            {
                var length = new byte[4];
                await networkStream.ReadAsync(length, 0, length.Length);
                count = BitConverter.ToInt32(length); 
            }

            var receivedArray = new byte[count];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < count &&
                   (read = await networkStream.ReadAsync(receivedArray, totalReceivedBytes, Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
                totalReceivedBytes += read;
            return receivedArray;
        }
    }
}
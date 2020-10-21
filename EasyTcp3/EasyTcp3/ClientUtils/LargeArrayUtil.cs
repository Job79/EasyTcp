using System;
using System.IO;
using System.IO.Compression;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with the SendLargeArray/ReceiveLargeArray functions 
    /// </summary>
    public static class LargeArrayUtil
    {
        /// <summary>
        /// Send array to the remote host
        /// Host can only receive an array when not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="array"></param>
        /// <param name="compression"></param>
        /// <param name="sendLengthPrefix">determines whether prefix with length of the data is send</param>
        public static void SendLargeArray(this EasyTcpClient client, byte[] array, bool compression = false,
            bool sendLengthPrefix = true)
        {
            if (!client.IsConnected()) throw new Exception("Could not send array: Client is not connected");

            var networkStream = client.Protocol.GetStream(client);
            var dataStream = compression ? new GZipStream(networkStream, CompressionMode.Compress, true) : networkStream;

            if (sendLengthPrefix) dataStream.Write(BitConverter.GetBytes(array.Length), 0, 4);
            dataStream.Write(array, 0, array.Length);
            
            if(compression) dataStream.Dispose();
        }

        /// <summary>
        /// Receive array from remote host
        /// Use this method only when not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="compression"></param>
        /// <param name="count">length of data, use prefix when 0</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static byte[] ReceiveLargeArray(this Message message, bool compression = false, int count = 0,
            int bufferSize = 1024)
        {
            if (message?.Client.IsConnected() != true) throw new Exception("Could not receive array: Client is not connected or message is null");

            using var networkStream = message.Client.Protocol.GetStream(message.Client);
            using var dataStream = compression ? new GZipStream(networkStream, CompressionMode.Decompress, true) : networkStream;

            // Get length from stream
            if (count == 0)
            {
                var length = new byte[4];
                dataStream.Read(length, 0, length.Length);
                count = BitConverter.ToInt32(length, 0);
            }

            var receivedArray = new byte[count];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < count &&
                   (read = dataStream.Read(receivedArray, totalReceivedBytes,
                       Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
                totalReceivedBytes += read;
            
            if(compression) dataStream.Dispose();
            return receivedArray;
        }
    }
}
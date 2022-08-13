using System;
using System.IO.Compression;

namespace EasyTcp4.ClientUtils
{
    public static class ArrayUtil
    {
        /// <summary>
        /// Send array to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="array"></param>
        /// <param name="compression">compress data using deflate if set to true</param>
        /// <param name="sendLengthPrefix">determines whether prefix with length of the data is send to the remote host</param>
        public static void SendArray(this EasyTcpClient client, byte[] array, bool compression = false, bool sendLengthPrefix = true)
        {
            if (client?.BaseSocket == null) throw new Exception("Could not send array: client is not connected");

            var networkStream = client.Protocol.GetStream(client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Compress, true) : networkStream;

            if (sendLengthPrefix) dataStream.Write(BitConverter.GetBytes(array.Length), 0, 4);
            dataStream.Write(array, 0, array.Length);

            if (compression) dataStream.Dispose();
        }

        /// <summary>
        /// Receive array from the remote host
        /// Use this method only when not client is not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="compression">compress data using deflate if set to true</param>
        /// <param name="count">length of data, use prefix when 0</param>
        /// <param name="bufferSize"></param>
        public static byte[] ReceiveArray(this Message message, bool compression = false, int count = 0, int bufferSize = 1024)
        {
            if (message?.Client?.BaseSocket == null) throw new Exception("Could not receive array: client is not connected or message is invalid");

            var networkStream = message.Client.Protocol.GetStream(message.Client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Decompress, true) : networkStream;

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
                   (read = dataStream.Read(receivedArray, totalReceivedBytes, Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
                totalReceivedBytes += read;

            if (compression) dataStream.Dispose();
            return receivedArray;
        }
    }
}

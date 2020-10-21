using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Class with the SendLargeArrayAsync/ReceiveLargeArrayAsync functions 
    /// </summary>
    public static class LargeArrayAsyncUtil
    {
        /// <summary>
        /// Send array to the remote host
        /// Host can only receive an array when not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="array"></param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <param name="sendLengthPrefix">determines whether prefix with length of the data is send</param>
        public static async Task SendLargeArrayAsync(this EasyTcpClient client, byte[] array,bool compression = false, bool sendLengthPrefix = true)
        {
            if(client?.BaseSocket == null) throw new Exception("Client is not connected");
            
            var networkStream = client.Protocol.GetStream(client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Compress, true) : networkStream;
            
            if(sendLengthPrefix) await dataStream.WriteAsync(BitConverter.GetBytes(array.Length),0, 4);
            await dataStream.WriteAsync(array,0,array.Length);
            
            if(compression) dataStream.Dispose();
        }

        /// <summary>
        /// Receive array from remote host
        /// Use this method only when not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <param name="count">length of data, use prefix when 0</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static async Task<byte[]> ReceiveLargeArrayAsync(this Message message, bool compression = false, int count = 0, int bufferSize = 1024)
        {
            if(message?.Client?.BaseSocket == null) throw new Exception("Client is not connected");
            
            var networkStream = message.Client.Protocol.GetStream(message.Client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Decompress, true) : networkStream;

            // Get length from stream
            if (count == 0)
            {
                var length = new byte[4];
                await dataStream.ReadAsync(length, 0, length.Length);
                count = BitConverter.ToInt32(length, 0); 
            }

            var receivedArray = new byte[count];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < count &&
                   (read = await dataStream.ReadAsync(receivedArray, totalReceivedBytes, Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
                totalReceivedBytes += read;
            
            if(compression) dataStream.Dispose();
            return receivedArray;
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Class with all the SendStreamAsync and ReceiveStreamAsync functions
    /// </summary>
    public static class StreamAsyncUtil
    {
        /// <summary>
        /// Send stream to the remote host
        /// Host can only receive a stream when not listening for incoming messages
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream">input stream</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not readable</exception>
        public static async Task SendStreamAsync(this EasyTcpClient client, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanRead) throw new InvalidDataException("Stream is not readable");

            await using var networkStream = client.Protocol.GetStream(client); 
            await networkStream.WriteAsync(BitConverter.GetBytes(stream.Length));

            var buffer = new byte[bufferSize];
            int read;

            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                await networkStream.WriteAsync(buffer, 0, read);
        }

        /// <summary>
        /// Receive stream from remote host
        /// Use this method only when not listening for incoming messages (In the OnReceive event)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream">output stream for receiving data</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static async Task ReceiveStreamAsync(this Message message, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanWrite) throw new InvalidDataException("Stream is not writable");

            await using var networkStream = message.Client.Protocol.GetStream(message.Client); 

            //Get length of stream
            var length = new byte[8];
            await networkStream.ReadAsync(length, 0, length.Length);
            var totalBytes = BitConverter.ToInt64(length);

            var buffer = new byte[bufferSize];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < totalBytes &&
                   (read = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, read);
                totalReceivedBytes += read;
            }
        }
    }
}
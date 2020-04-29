using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils.Async
{
    public static class StreamAsyncUtil
    {
        /// <summary>
        /// Send a stream to the remote host.
        /// Because the host can only receive a stream in the OnReceive event, first send a normal message (See examples)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream">Stream to send to the host</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">Stream is not readable</exception>
        public static async Task SendStreamAsync(this EasyTcpClient client, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanRead) throw new InvalidDataException("Stream is not readable");
            
            await using var networkStream = new NetworkStream(client.BaseSocket);
            await networkStream.WriteAsync(BitConverter.GetBytes(stream.Length));

            var buffer = new byte[bufferSize];
            int read;

            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                await networkStream.WriteAsync(buffer, 0, read);
        }
        
        /// <summary>
        /// Receive a stream from a remote host,
        /// This can only be done in an OnReceive event.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream">Stream to write receiving stream to</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">Stream is not writable</exception>
        public static async Task ReceiveStreamAsync(this Message message, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanWrite) throw new InvalidDataException("Stream is not writable");
            
            await using var networkStream = new NetworkStream(message.Client.BaseSocket);
            
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
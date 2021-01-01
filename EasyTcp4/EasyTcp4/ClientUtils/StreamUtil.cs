using System;
using System.IO;
using System.IO.Compression;

namespace EasyTcp4.ClientUtils
{
    public static class StreamUtil
    {
        /// <summary>
        /// Send stream to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream">input stream</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        /// <param name="sendLengthPrefix">determines whether prefix with length of the data is send to the remote host</param>
        /// <param name="bufferSize"></param>
        public static void SendStream(this EasyTcpClient client, Stream stream,
                bool compression = false, bool sendLengthPrefix = true, int bufferSize = 1024)
        {
            if (client?.BaseSocket == null) throw new Exception("Could not send stream: client is not connected");
            if (!stream.CanRead) throw new InvalidDataException("Could not send stream: stream is not readable");

            var networkStream = client.Protocol.GetStream(client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Compress, true) : networkStream;

            if (sendLengthPrefix) dataStream.Write(BitConverter.GetBytes(stream.Length), 0, 8);

            var buffer = new byte[bufferSize];
            int read;

            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                dataStream.Write(buffer, 0, read);

            if (compression) dataStream.Dispose();
        }

        /// <summary>
        /// Receive stream from the remote host
        /// Use this method only when not client is not listening for incoming messages (Inside OnReceive event handlers)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream">output stream for receiving data</param>
        /// <param name="compression">compress data using deflate if set to true</param>
        /// <param name="count">length of receiving stream, use prefix when 0</param>
        /// <param name="bufferSize"></param>
        public static void ReceiveStream(this Message message, Stream stream,
                bool compression = false, long count = 0, int bufferSize = 1024)
        {
            if (message?.Client?.BaseSocket == null) throw new Exception("Could not send stream: client is not connected or mesage is invalid");
            if (!stream.CanWrite) throw new InvalidDataException("Could not send stream: stream is not writable");

            var networkStream = message.Client.Protocol.GetStream(message.Client);
            var dataStream = compression ? new DeflateStream(networkStream, CompressionMode.Decompress, true) : networkStream;

            //Get length of stream
            if (count == 0)
            {
                var length = new byte[8];
                dataStream.Read(length, 0, length.Length);
                count = BitConverter.ToInt64(length, 0);
            }

            var buffer = new byte[bufferSize];
            long totalReceivedBytes = 0;
            int read;

            while (totalReceivedBytes < count &&
                   (read = dataStream.Read(buffer, 0, (int)Math.Min(bufferSize, count - totalReceivedBytes))) > 0)
            {
                stream.Write(buffer, 0, read);
                totalReceivedBytes += read;
            }

            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            if (compression) dataStream.Dispose();
        }
    }
}

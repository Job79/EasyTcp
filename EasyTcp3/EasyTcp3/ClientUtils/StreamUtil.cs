using System;
using System.IO;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with all the SendStream and ReceiveStream functions
    /// </summary>
    public static class StreamUtil
    {
        /// <summary>
        /// Send stream to the remote host
        /// Host can only receive a stream when not listening for incoming messages
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream">input stream</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not readable</exception>
        public static void SendStream(this EasyTcpClient client, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanRead) throw new InvalidDataException("Stream is not readable");

            using var networkStream = client.Protocol.GetStream(client); 
            networkStream.Write(BitConverter.GetBytes(stream.Length));

            var buffer = new byte[bufferSize];
            int read;

            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                networkStream.Write(buffer, 0, read);
        }

        /// <summary>
        /// Receive stream from remote host
        /// Use this method only when not listening for incoming messages (In the OnReceive event)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream">output stream for receiving data</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static void ReceiveStream(this Message message, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanWrite) throw new InvalidDataException("Stream is not writable");

            using var networkStream = message.Client.Protocol.GetStream(message.Client); 

            //Get length of stream
            var length = new byte[8];
            networkStream.Read(length, 0, length.Length);
            var totalBytes = BitConverter.ToInt64(length);

            var buffer = new byte[bufferSize];
            int read, totalReceivedBytes = 0;

            while (totalReceivedBytes < totalBytes &&
                   (read = networkStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, read);
                totalReceivedBytes += read;
            }
        }
    }
}
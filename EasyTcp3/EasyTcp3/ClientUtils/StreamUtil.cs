using System;
using System.IO;
using System.Net.Sockets;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Functions to send or receive a stream to/from a remote host
    /// </summary>
    public static class StreamUtil
    {
        /// <summary>
        /// Send a stream to the remote host.
        /// Because the host can only receive a stream in the OnReceive event, first send a normal message (See fileserver/fileclient in examples)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream">stream to send to the host</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not readable</exception>
        public static void SendStream(this EasyTcpClient client, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanRead) throw new InvalidDataException("Stream is not readable");
            
            using var networkStream = new NetworkStream(client.BaseSocket);
            networkStream.Write(BitConverter.GetBytes(stream.Length));

            var buffer = new byte[bufferSize];
            int read;

            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                networkStream.Write(buffer, 0, read);
        }

        /// <summary>
        /// Receive a stream from a remote host,
        /// This can only be used in an OnReceive event while the BeginReceive isn't active.
        /// (Do not use this function on an message from SendAndGetReply!)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream">stream to write receiving stream to</param>
        /// <param name="bufferSize"></param>
        /// <exception cref="InvalidDataException">stream is not writable</exception>
        public static void ReceiveStream(this Message message, Stream stream, int bufferSize = 1024)
        {
            if (!stream.CanWrite) throw new InvalidDataException("Stream is not writable");
            
            using var networkStream = new NetworkStream(message.Client.BaseSocket);
            
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
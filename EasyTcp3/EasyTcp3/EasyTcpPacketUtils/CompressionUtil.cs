using System.IO;
using System.IO.Compression;

namespace EasyTcp3.EasyTcpPacketUtils
{
    /// <summary>
    /// Class that contains functions for compressing packages
    /// </summary>
    public static class CompressionUtil
    {
        /// <summary>
        /// Compress byte[] of data using GZIP
        /// </summary>
        /// <param name="data">uncompressed data</param>
        /// <returns>compressed data</returns>
        public static byte[] Compress(byte[] data)
        {
            if(data == null || data.Length <= 0) throw new InvalidDataException("Could not compress data: data array is empty");
            using var compressedStream = new MemoryStream();
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }

        /// <summary>
        /// Decompress byte[] of data using GZIP
        /// </summary>
        /// <param name="data">compressed data</param>
        /// <returns>decompressed data</returns>
        public static byte[] Decompress(byte[] data)
        {
            if(data == null || data.Length <= 0) throw new InvalidDataException("Could not decompress data: data array is empty");
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
        
        /// <summary>
        /// Determines whether the receive data is compressed with GZIP
        /// </summary>
        /// <returns>true if compressed</returns>
        public static bool IsCompressed(byte[] data) => data.Length > 4 && data[0] == 31 && data[1] == 139;
        
        /// <summary>
        /// Determines whether the receive data is compressed with GZIP
        /// </summary>
        /// <returns>true if compressed</returns>
        public static bool IsCompressed(this IEasyTcpPacket packet) => IsCompressed(packet.Data);
       
        /// <summary>
        /// Compress package if not already compressed
        /// </summary>
        /// <param name="packet">packet to compress</param>
        /// <typeparam name="T">packet type</typeparam>
        /// <returns>instance of packet parameter</returns>
        public static T Compress<T>(this T packet) where T : IEasyTcpPacket
        {
            if (packet.IsCompressed()) return packet;
            packet.Data = Compress(packet.Data);
            return packet;
        }

        /// <summary>
        /// Decompress package if data is compressed
        /// </summary>
        /// <param name="packet">decompressed package</param>
        /// <typeparam name="T">packet type</typeparam>
        /// <returns>instance of packet parameter</returns>
        public static T Decompress<T>(this T packet) where T : IEasyTcpPacket
        {
            if (!packet.IsCompressed()) return packet;
            try
            {
                packet.Data = Decompress(packet.Data);
            }
            catch
            {
                //Ignore error, data is invalid
            }
            return packet;
        } 
    }
}
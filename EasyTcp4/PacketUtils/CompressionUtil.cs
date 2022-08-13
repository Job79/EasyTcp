using System.IO;
using System.IO.Compression;

namespace EasyTcp4.PacketUtils
{
    public static class CompressionUtil
    {
        /// <summary>
        /// Compress byte[] of data using deflate and add magic number
        /// </summary>
        /// <param name="data">uncompressed data</param>
        /// <returns>magic number + compressed data</returns>
        public static byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0) throw new InvalidDataException("Could not compress data: data array is empty");
            using var compressedStream = new MemoryStream();
            compressedStream.Write(new byte[] { 0x78, 0x9C }, 0, 2);
            var deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress);
            deflateStream.Write(data, 0, data.Length);
            deflateStream.Dispose();
            return compressedStream.ToArray();
        }

        /// <summary>
        /// Decompress byte[] of data using deflate
        /// </summary>
        /// <param name="data">magic number + compressed data</param>
        /// <returns>decompressed data</returns>
        public static byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length == 0) throw new InvalidDataException("Could not decompress data: data array is empty");
            using var compressedStream = new MemoryStream(data);
            compressedStream.Seek(2, SeekOrigin.Current);
            using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            deflateStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }

        /// <summary>
        /// Determines whether the receive data is compressed with deflate by looking for the magic number
        /// </summary>
        /// <returns>true if compressed</returns>
        public static bool IsCompressed(byte[] d) => d.Length > 2 && d[0] == 0x78 && d[1] == 0x9C;

        /// <summary>
        /// Determines whether the receive data is compressed with deflate by looking for the magic number
        /// </summary>
        /// <returns>true if compressed</returns>
        public static bool IsCompressed(this IEasyPacket packet) => IsCompressed(packet.Data);

        /// <summary>
        /// Compress package if not already compressed
        /// </summary>
        /// <param name="packet">packet to compress</param>
        /// <typeparam name="T">packet type</typeparam>
        /// <returns>compressed package</returns>
        public static T Compress<T>(this T packet) where T : IEasyPacket
        {
            if (packet.IsCompressed()) return packet;
            packet.Data = Compress(packet.Data);
            return packet;
        }

        /// <summary>
        /// Decompress package if data is compressed
        /// </summary>
        /// <param name="packet">compressed package</param>
        /// <typeparam name="T">packet type</typeparam>
        /// <returns>decompressed package</returns>
        public static T Decompress<T>(this T packet) where T : IEasyPacket
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

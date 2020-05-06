using System.IO;
using System.IO.Compression;

namespace EasyTcp3
{
    public static class Compression
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
    }
}
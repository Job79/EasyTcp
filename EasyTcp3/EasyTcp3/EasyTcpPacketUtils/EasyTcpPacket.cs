using System;
using System.Linq;
using System.Text;

namespace EasyTcp3.EasyTcpPacketUtils
{
    public static class EasyTcpPacket // FromDO: Add documentation
    {
        public static T From<T>(byte[] data, bool compression = false) where T : IEasyTcpPacket, new()
        {
            if (compression) return new T {Data = Compression.Compress(data)};
            else return new T {Data = data};
        }

        public static T From<T>(params byte[][] data) where T : IEasyTcpPacket, new()
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create packet: Data array is empty");

            // Calculate length of new data
            var messageLength = data.Sum(t => t?.Length ?? 0);
            byte[] newData = new byte[messageLength];

            int offset = 0;
            foreach (var d in data)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, newData, offset, d.Length);
                offset += d.Length;
            }

            return From<T>(newData);
        }

        public static T From<T>(ushort data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(short data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(uint data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(int data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(ulong data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(long data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(double data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(bool data) where T : IEasyTcpPacket, new()
            => From<T>(BitConverter.GetBytes(data));

        public static T From<T>(string data, Encoding encoding = null, bool compression = false)
            where T : IEasyTcpPacket, new()
            => From<T>((encoding ?? Encoding.UTF8).GetBytes(data), compression);


        /// <summary>
        /// Determines whether the receive data is compressed using the magic no of GZIP (1f 2b)
        /// </summary>
        /// <returns></returns>
        public static bool IsCompressed(this IEasyTcpPacket packet) => packet.Data.Length > 4 && packet.Data[0] == 31 && packet.Data[1] == 139;
        
        public static T Compress<T>(this T packet) where T : IEasyTcpPacket
        {
            if (packet.IsCompressed()) return packet;
            packet.Data = Compression.Compress(packet.Data);
            return packet;
        }

        public static T Decompress<T>(this T packet) where T : IEasyTcpPacket
        {
            if (!packet.IsCompressed()) return packet;
            try
            {
                packet.Data = Compression.Compress(packet.Data);
            }
            catch
            {
                //Ignore error, data isn't compressed or invalid
            }
            return packet;
        }
        
    }
}
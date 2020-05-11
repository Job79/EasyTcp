using System;
using System.Linq;
using System.Text;

namespace EasyTcp3.EasyTcpPacketUtils
{
    public static class EasyTcpPacket // TODO: Add documentation
    {
        public static T To<T>(byte[] data, bool compression = false) where T : IEasyTcpPacket, new()
        {
            if (compression) return new T {Data = Compression.Compress(data)};
            else return new T {Data = data};
        }

        public static T To<T>(params byte[][] data) where T : IEasyTcpPacket, new()
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

            return To<T>(newData);
        }

        public static T To<T>(ushort data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(short data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(uint data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(int data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(ulong data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(long data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(double data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(bool data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        public static T To<T>(string data, Encoding encoding = null, bool compression = false)
            where T : IEasyTcpPacket, new()
            => To<T>((encoding ?? Encoding.UTF8).GetBytes(data), compression);


        public static T Compress<T>(this T packet) where T : IEasyTcpPacket
        {
            packet.Data = Compression.Compress(packet.Data);
            return packet;
        }

        public static T Decompress<T>(this T packet) where T : IEasyTcpPacket
        {
            packet.Data = Compression.Compress(packet.Data);
            return packet;
        }
    }
}
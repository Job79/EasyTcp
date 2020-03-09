using System;
using System.Text;

namespace EasyTcp3
{
    public class Message
    {
        public readonly byte[] Data;
        public readonly EasyTcpClient Client;

        public Message(byte[] data, EasyTcpClient client)
        {
            Data = data;
            Client = client;
        }

        public bool IsValidUShort() => Data.Length == 2;
        public bool IsValidShort() => Data.Length == 2;
        public bool IsValidUInt() => Data.Length == 4;
        public bool IsValidInt() => Data.Length == 4;
        public bool IsValidULong() => Data.Length == 8;
        public bool IsValidLong() => Data.Length == 8;
        public bool IsValidDouble() => Data.Length == 8;
        public bool IsValidBool() => Data.Length == 1;

        public ushort ToUShort() => BitConverter.ToUInt16(Data);
        public short ToShort() => BitConverter.ToInt16(Data);
        public uint ToUInt() => BitConverter.ToUInt32(Data);
        public int ToInt() => BitConverter.ToInt16(Data);
        public ulong ToULong() => BitConverter.ToUInt64(Data);
        public long ToLong() => BitConverter.ToInt64(Data);
        public double ToDouble() => BitConverter.ToDouble(Data);
        public bool ToBool() => BitConverter.ToBoolean(Data);
        public string ToString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(Data);
    }
}
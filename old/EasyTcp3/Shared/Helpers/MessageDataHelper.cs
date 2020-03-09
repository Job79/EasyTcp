using System;

namespace EasyTcp3.Helpers
{
    public static class MessageDataHelper
    {
        public static bool IsValidUShort(this Message m) => m.Data.Length == 2;
        public static bool IsValidShort(this Message m) => m.Data.Length == 2;
        public static bool IsValidUInt(this Message m) => m.Data.Length == 4;
        public static bool IsValidInt(this Message m) => m.Data.Length == 4;
        public static bool IsValidULong(this Message m) => m.Data.Length == 8;
        public static bool IsValidLong(this Message m) => m.Data.Length == 8;
        public static bool IsValidDouble(this Message m) => m.Data.Length == 8;
        public static bool IsValidBool(this Message m) => m.Data.Length == 1;
        
        public static ushort ToUShort(this Message m) => BitConverter.ToUInt16(m.Data);
        public static short ToShort(this Message m) => BitConverter.ToInt16(m.Data);
        public static uint ToUInt(this Message m) => BitConverter.ToUInt32(m.Data);
        public static int ToInt(this Message m) => BitConverter.ToInt16(m.Data);
        public static ulong ToULong(this Message m) => BitConverter.ToUInt64(m.Data);
        public static long ToLong(this Message m) => BitConverter.ToInt64(m.Data);
        public static double ToDouble(this Message m) => BitConverter.ToDouble(m.Data);
        public static bool ToBool(this Message m) => BitConverter.ToBoolean(m.Data);
    }
}
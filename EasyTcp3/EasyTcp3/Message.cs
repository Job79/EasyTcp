using System;
using System.Text;

namespace EasyTcp3
{
    public class Message
    {
        /// <summary>
        /// Received data as byte[]
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// Client that did receive this message
        /// </summary>
        public readonly EasyTcpClient Client;

        /// <summary></summary>
        /// <param name="data">Received data as byte[]</param>
        /// <param name="client">client that did receive this message</param>
        public Message(byte[] data, EasyTcpClient client)
        {
            Data = data;
            Client = client;
        }

        /// <summary>
        /// Determines whether the received data is a valid UShort 
        /// </summary>
        /// <returns></returns>
        public bool IsValidUShort() => Data.Length == 2;
        /// <summary>
        /// Determines whether the received data is a valid Short 
        /// </summary>
        /// <returns></returns>
        public bool IsValidShort() => Data.Length == 2;
        /// <summary>
        /// Determines whether the received data is a valid UInt 
        /// </summary>
        /// <returns></returns>
        public bool IsValidUInt() => Data.Length == 4;
        /// <summary>
        /// Determines whether the received data is a valid Int 
        /// </summary>
        /// <returns></returns>
        public bool IsValidInt() => Data.Length == 4;
        /// <summary>
        /// Determines whether the received data is a valid ULong 
        /// </summary>
        /// <returns></returns>
        public bool IsValidULong() => Data.Length == 8;
        /// <summary>
        /// Determines whether the received data is a valid Long 
        /// </summary>
        /// <returns></returns>
        public bool IsValidLong() => Data.Length == 8;
        /// <summary>
        /// Determines whether the received data is a valid Double 
        /// </summary>
        /// <returns></returns>
        public bool IsValidDouble() => Data.Length == 8;
        /// <summary>
        /// Determines whether the received data is a valid Bool 
        /// </summary>
        /// <returns></returns>
        public bool IsValidBool() => Data.Length == 1;

        /// <summary>
        /// Received data as UShort
        /// </summary>
        /// <returns>data as UShort</returns>
        public ushort ToUShort() => BitConverter.ToUInt16(Data);
        /// <summary>
        /// Received data as Short
        /// </summary>
        /// <returns>data as Short</returns>
        public short ToShort() => BitConverter.ToInt16(Data);
        /// <summary>
        /// Received data as UInt
        /// </summary>
        /// <returns>data as UInt</returns>
        public uint ToUInt() => BitConverter.ToUInt32(Data);
        /// <summary>
        /// Received data as Int
        /// </summary>
        /// <returns>data as Int</returns>
        public int ToInt() => BitConverter.ToInt16(Data);
        /// <summary>
        /// Received data as ULong
        /// </summary>
        /// <returns>data as ULong</returns>
        public ulong ToULong() => BitConverter.ToUInt64(Data);
        /// <summary>
        /// Received data as Long
        /// </summary>
        /// <returns>data as Long</returns>
        public long ToLong() => BitConverter.ToInt64(Data);
        /// <summary>
        /// Received data as Double
        /// </summary>
        /// <returns>data as Double</returns>
        public double ToDouble() => BitConverter.ToDouble(Data);
        /// <summary>
        /// Received data as Bool
        /// </summary>
        /// <returns>data as Bool</returns>
        public bool ToBool() => BitConverter.ToBoolean(Data);
        /// <summary>
        /// Received data as String
        /// </summary>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <returns>Data as string</returns>
        public string ToString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(Data);
    }
}
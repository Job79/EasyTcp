using System;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3
{
    /// <summary>
    /// Class that passed by the OnDataReceive event handler
    /// Contains received data, socket and simple functions to convert data
    /// </summary>
    public class Message : IEasyTcpPacket 
    {
        /// <summary>
        /// Received data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Receiver of this message
        /// </summary>
        public readonly EasyTcpClient Client;

        /// <summary></summary>
        /// <param name="data">received data</param>
        /// <param name="client">receiver</param>
        public Message(byte[] data, EasyTcpClient client = null)
        {
            Data = data;
            Client = client;
        }
        /// <summary>
        /// </summary>
        public Message() { }

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
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <returns>data as string</returns>
        public string ToString(Encoding encoding) => (encoding ?? Encoding.UTF8).GetString(Data);

        /// <summary>
        /// Receive data as string decoded with UTF8
        /// </summary>
        /// <returns>data as string</returns>
        public override string ToString() => ToString(Encoding.UTF8);

        /// <summary>
        /// Received data as a custom IEasyTcpPacket 
        /// </summary>
        /// <typeparam name="T">Packet type</typeparam>
        /// <returns>data as custom IEasyTcpPacket</returns>
        public T ToPacket<T>() where T : IEasyTcpPacket, new() => EasyTcpPacket.To<T>(Data);
    }
}
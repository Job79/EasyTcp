using System;
using System.Collections.Generic;
using System.Text;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3
{
    /// <summary>
    /// Parameter of the OnDataReceive events
    /// Represents received data
    /// </summary>
    public class Message : IEasyTcpPacket
    {
        /// <summary>
        /// Received data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Server: Sender of message
        /// Client: Receiver of message
        /// </summary>
        public readonly EasyTcpClient Client;

        /// <summary></summary>
        public Message() { }

        /// <summary></summary>
        /// <param name="data">received data</param>
        /// <param name="client"></param>
        public Message(byte[] data, EasyTcpClient client = null)
        {
            Data = data;
            Client = client;
        }
        
        /// <summary>
        /// MetaData of message, empty when not used by protocol
        /// </summary>
        public Dictionary<string, object> MetaData 
        {
            get => (_metaData ??= new Dictionary<string, object>());
            set => _metaData = value;
        }
        
        private Dictionary<string, object> _metaData;

        /// <summary>
        /// Determines whether received data is a valid UShort 
        /// </summary>
        /// <returns></returns>
        public bool IsValidUShort() => Data.Length == 2;

        /// <summary>
        /// Determines whether received data is a valid Short 
        /// </summary>
        /// <returns></returns>
        public bool IsValidShort() => Data.Length == 2;

        /// <summary>
        /// Determines whether received data is a valid UInt 
        /// </summary>
        /// <returns></returns>
        public bool IsValidUInt() => Data.Length == 4;

        /// <summary>
        /// Determines whether received data is a valid Int 
        /// </summary>
        /// <returns></returns>
        public bool IsValidInt() => Data.Length == 4;

        /// <summary>
        /// Determines whether received data is a valid ULong 
        /// </summary>
        /// <returns></returns>
        public bool IsValidULong() => Data.Length == 8;

        /// <summary>
        /// Determines whether received data is a valid Long 
        /// </summary>
        /// <returns></returns>
        public bool IsValidLong() => Data.Length == 8;

        /// <summary>
        /// Determines whether received data is a valid Double 
        /// </summary>
        /// <returns></returns>
        public bool IsValidDouble() => Data.Length == 8;

        /// <summary>
        /// Determines whether received data is a valid Bool 
        /// </summary>
        /// <returns></returns>
        public bool IsValidBool() => Data.Length == 1;

        /// <summary>
        /// Convert data to UShort
        /// </summary>
        /// <returns>data as UShort</returns>
        public ushort ToUShort() => BitConverter.ToUInt16(Data, 0);

        /// <summary>
        /// Convert data to Short
        /// </summary>
        /// <returns>data as Short</returns>
        public short ToShort() => BitConverter.ToInt16(Data, 0);

        /// <summary>
        /// Convert data to UInt
        /// </summary>
        /// <returns>data as UInt</returns>
        public uint ToUInt() => BitConverter.ToUInt32(Data, 0);

        /// <summary>
        /// Convert data to Int
        /// </summary>
        /// <returns>data as Int</returns>
        public int ToInt() => BitConverter.ToInt16(Data, 0);

        /// <summary>
        /// Convert data to ULong
        /// </summary>
        /// <returns>data as ULong</returns>
        public ulong ToULong() => BitConverter.ToUInt64(Data, 0);

        /// <summary>
        /// Convert data to Long
        /// </summary>
        /// <returns>data as Long</returns>
        public long ToLong() => BitConverter.ToInt64(Data, 0);

        /// <summary>
        /// Convert data to Double
        /// </summary>
        /// <returns>data as Double</returns>
        public double ToDouble() => BitConverter.ToDouble(Data, 0);

        /// <summary>
        /// Convert data to Bool
        /// </summary>
        /// <returns>data as Bool</returns>
        public bool ToBool() => BitConverter.ToBoolean(Data, 0);

        /// <summary>
        /// Convert data to String
        /// </summary>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <returns>data as string</returns>
        public string ToString(Encoding encoding) => (encoding ?? Encoding.UTF8).GetString(Data);

        /// <summary>
        /// Convert data to string with UTF8
        /// </summary>
        /// <returns>data as string</returns>
        public override string ToString() => ToString(Encoding.UTF8);

        /// <summary>
        /// Convert data to custom IEasyTcpPacket 
        /// </summary>
        /// <typeparam name="T">Packet type</typeparam>
        /// <returns>data as custom IEasyTcpPacket</returns>
        public T ToPacket<T>() where T : IEasyTcpPacket, new() => EasyTcpPacket.From<T>(Data);

        /// <summary>
        /// Deserialize object from byte[] 
        /// </summary>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            try
            {
                return (T) Client.Deserialize(Data, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
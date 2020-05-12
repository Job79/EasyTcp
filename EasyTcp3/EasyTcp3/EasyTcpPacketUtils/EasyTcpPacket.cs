using System;
using System.Linq;
using System.Text;

namespace EasyTcp3.EasyTcpPacketUtils
{
    /// <summary>
    /// Class with functions for creating new EasyTcpPackets
    /// </summary>
    public static class EasyTcpPacket
    {
        /// <summary>
        /// Create package with a byte array as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <param name="compression">if true package will be compressed</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(byte[] data, bool compression = false) where T : IEasyTcpPacket, new()
            => compression ? new T {Data = data}.Compress() : new T {Data = data};

        /// <summary>
        /// Create package with multiple byte arrays as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        /// <exception cref="ArgumentException">could not create packet: Data array is empty</exception>
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
        
        /// <summary>
        /// Create package with an ushort as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(ushort data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        /// <summary>
        /// Create package with a short as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(short data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with an uint as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(uint data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with a int as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(int data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with an ulong as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(ulong data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with a long as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(long data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with a double as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(double data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Create package with a bool as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(bool data) where T : IEasyTcpPacket, new()
            => To<T>(BitConverter.GetBytes(data));

        /// <summary>
        /// Create package with a string as its data 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <param name="encoding">encoding type (Default: UTF8)</param>
        /// <param name="compression">if true package will be compressed</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T To<T>(string data, Encoding encoding = null, bool compression = false)
            where T : IEasyTcpPacket, new()
            => To<T>((encoding ?? Encoding.UTF8).GetBytes(data), compression);
    }
}
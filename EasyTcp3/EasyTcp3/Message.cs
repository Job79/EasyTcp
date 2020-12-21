using System.Collections.Generic;
using System.Text;
using EasyTcp3.PacketUtils;

namespace EasyTcp3
{
    /// <summary>
    /// Class that represents received data
    /// </summary>
    public class Message : IEasyTcpPacket
    {
        /// <summary>
        /// Received data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Server: Sender of message
        /// Client: Current client 
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
        /// List with MetaData of received message
        /// Available to custom protocols to store information.
        /// </summary>
        public Dictionary<string, object> MetaData 
        {
            get => _metaData ??= new Dictionary<string, object>();
            set => _metaData = value;
        }
        
        private Dictionary<string, object> _metaData;

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
        /// <typeparam name="T">packet type</typeparam>
        /// <returns>data as custom IEasyTcpPacket</returns>
        public T ToPacket<T>() where T : IEasyTcpPacket, new()
            => new T { Data = this.Data };

        /// <summary>
        /// Deserialize byte[] to custom object
        /// </summary>
        /// <returns>byte[] as custom object</returns>
        public T Deserialize<T>()
        {
            try { return (T) Client.Deserialize(Data, typeof(T)); }
            catch { return default; }
        }
    }
}

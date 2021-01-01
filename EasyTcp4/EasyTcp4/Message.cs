using System.Collections.Generic;
using System.Text;
using EasyTcp4.PacketUtils;

namespace EasyTcp4
{
    /// <summary>
    /// Class that represents received data
    /// </summary>
    public class Message : IEasyPacket
    {
        /// <summary>
        /// Received data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Server: sender of message
        /// Client: current client 
        /// </summary>
        public readonly EasyTcpClient Client;

        /// <summary></summary>
        public Message() { }

        /// <summary></summary>
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
        public string ToString(Encoding encoding) => encoding.GetString(Data);

        /// <summary>
        /// Convert data to string with UTF8
        /// </summary>
        public override string ToString() => ToString(Encoding.UTF8);

        /// <summary>
        /// Convert data to custom IEasyTcpPacket 
        /// </summary>
        /// <returns>data as custom IEasyTcpPacket</returns>
        public T ToPacket<T>() where T : IEasyPacket, new()
            => new T { Data = this.Data };

        /// <summary>
        /// Deserialize data to custom object
        /// </summary>
        /// <returns>data as custom object</returns>
        public T To<T>()
        {
            try { return (T)Client.Deserialize(Data, typeof(T)); }
            catch { return default; }
        }
    }
}

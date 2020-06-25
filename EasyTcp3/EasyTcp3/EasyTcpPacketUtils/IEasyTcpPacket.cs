namespace EasyTcp3.EasyTcpPacketUtils
{
    /// <summary>
    /// Interface used by multiple EasyTcpFunctions,
    /// implement when a class needs to be send over the network
    /// </summary>
    public interface IEasyTcpPacket
    {
        /// <summary>
        /// get => return class as byte[]
        /// set => create class from byte[]
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// Create package from byte array 
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <param name="compression">compress data using GZIP if set to true</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T From<T>(byte[] data, bool compression = false) where T : IEasyTcpPacket, new()
            => compression ? new T {Data = data}.Compress() : new T {Data = data};
    }
}
namespace EasyTcp3.EasyTcpPacketUtils
{
    public static class EasyTcpPacket
    {
        /// <summary>
        /// Create package from byte array
        /// </summary>
        /// <param name="data">data of new package</param>
        /// <param name="compression">compress data using Deflate if set to true</param>
        /// <typeparam name="T">package type</typeparam>
        /// <returns>new package</returns>
        public static T From<T>(byte[] data, bool compression = false) where T : IEasyTcpPacket, new()
            => compression ? new T {Data = data}.Compress() : new T {Data = data};
    }
}
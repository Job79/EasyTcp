namespace EasyTcp3
{
    /// <summary>
    /// Interface used by multiple EasyTcpFunctions,
    /// implement when a class needs to be send over TCP
    /// </summary>
    public interface IEasyTcpPacket
    {
        /// <summary>
        /// get => return class as byte[]
        /// set => create class from byte[]
        /// </summary>
        public byte[] Data { get; set; }
    }
}
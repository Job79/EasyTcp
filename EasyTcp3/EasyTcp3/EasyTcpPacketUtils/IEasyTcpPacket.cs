namespace EasyTcp3.EasyTcpPacketUtils
{
    /// <summary>
    /// Interface used by multiple EasyTcp functions,
    /// implement when a class needs to be send over the network and standard serialization is to slow / not possible
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
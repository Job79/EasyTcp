namespace EasyTcp4.PacketUtils
{
    /// <summary>
    /// Interface used by multiple EasyTcp functions,
    /// implement when a class needs to be send over the network and standard serialization is too slow / not possible
    /// </summary>
    public interface IEasyPacket
    {
        /// <summary>
        /// get => return class as byte[]
        /// set => create class from byte[]
        /// </summary>
        public byte[] Data { get; set; }
    }
}

namespace EasyTcp3
{
    public interface IEasyTcpPacket
    {
        public byte[] ToArray();
        public void FromArray(byte[] data);
    }
}
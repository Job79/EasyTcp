using EasyTcp3;

namespace EasyTcp.Encryption
{
    public class EncryptedPacket : Message, IEasyTcpPacket
    {
        public EncryptedPacket(byte[] data, EasyTcpClient client) : base(data, client) { }

        public byte[] ToArray() =>base.Data;
        public void FromArray(byte[] data) => base.Data = data;
    }
}
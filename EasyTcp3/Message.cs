namespace EasyTcp3
{
    public class Message
    {
        public byte[] Data;
        public Message(byte[] data) => Data = data;
    }
}
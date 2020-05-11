using EasyTcp3;

namespace EasyTcp.Encryption
{
    public static class EncryptionUtil 
    {
        public static Message Encrypt(this Message message, EasyEncrypt encryption)
        {
            message.Data = encryption.Encrypt(message.Data);
            return message;
        }

        public static Message Decrypt(this Message message, EasyEncrypt encryption)
        {
            message.Data = encryption.Decrypt(message.Data);
            return message;
        }
    }
}
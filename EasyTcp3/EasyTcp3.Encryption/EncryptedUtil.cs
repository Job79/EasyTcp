using EasyEncrypt2;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp.Encryption
{
    /// <summary>
    /// Class with functions for encrypting packages
    /// </summary>
    public static class EncryptionUtil 
    {
        /// <summary>
        /// Encrypt message with EasyEncrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encryption">instance of easyEncrypt class</param>
        /// <returns>encrypted data</returns>
        public static T Encrypt<T>(this T data, EasyEncrypt encryption) where T : IEasyTcpPacket 
        {
            data.Data = encryption.Encrypt(data.Data);
            return data;
        }

        /// <summary>
        /// Decrypt message with EasyEncrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encryption">instance of easyEncrypt class</param>
        /// <returns>decrypted data</returns>
        public static T Decrypt<T>(this T data, EasyEncrypt encryption) where T : IEasyTcpPacket
        {
            data.Data = encryption.Decrypt(data.Data);
            return data;
        }
    }
}

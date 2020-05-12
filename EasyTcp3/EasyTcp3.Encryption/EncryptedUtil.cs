using EasyTcp3;

namespace EasyTcp.Encryption
{
    /// <summary>
    /// Functions for encrypting packages
    /// </summary>
    public static class EncryptionUtil 
    {
        /// <summary>
        /// Encrypt message with EasyEncrypt instance
        /// </summary>
        /// <param name="data">data to encrypt, instance get encrypted + returned</param>
        /// <param name="encryption">instance of encryption class</param>
        /// <returns>itself, encrypted data</returns>
        public static T Encrypt<T>(this T data, EasyEncrypt encryption) where T : IEasyTcpPacket 
        {
            data.Data = encryption.Encrypt(data.Data);
            return data;
        }

        /// <summary>
        /// Encrypt message with EasyEncrypt instance 
        /// </summary>
        /// <param name="data">data to encrypt, instance get encrypted + returned</param>
        /// <param name="encryption">instance of a encryption class</param>
        /// <returns>itself, encrypted data</returns>
        public static T Decrypt<T>(this T data, EasyEncrypt encryption) where T : IEasyTcpPacket
        {
            data.Data = encryption.Decrypt(data.Data);
            return data;
        }
    }
}

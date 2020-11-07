using EasyEncrypt2;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Encryption.Protocols.Tcp;

namespace EasyTcp3.Encryption
{
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

        /// <summary>
        /// Shortcut for enabling encryption 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="encrypt"></param>
        public static T UseEncryption<T>(this T client, EasyEncrypt encrypt) where T : EasyTcpClient
        {
            client.Protocol = new EncryptedPrefixLengthProtocol(encrypt);
            return client;
        }
        
        /// <summary>
        /// Shortcut for enabling encryption 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="encrypt"></param>
        public static T UseServerEncryption<T>(this T server, EasyEncrypt encrypt) where T : EasyTcpServer
        {
            server.Protocol = new EncryptedPrefixLengthProtocol(encrypt);
            return server;
        }
    }
}

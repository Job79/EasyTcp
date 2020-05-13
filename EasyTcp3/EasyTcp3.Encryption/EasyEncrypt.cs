using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasyTcp.Encryption
{
    /// <summary>
    /// Class that provides basic encryption functionality
    /// </summary>
    public class EasyEncrypt : IDisposable
    {
        /// <summary>
        /// Algorithm that is used for encrypting and decrypting data
        /// </summary> 
        private readonly SymmetricAlgorithm _algorithm;
        
        /// <summary>
        /// </summary>
        /// <param name="algorithm">algorithm used for encryption, Aes if null</param>
        /// <param name="key">key used for encryption, secure random if null</param>
        /// <exception cref="ArgumentException">can't create EasyEncrypt: key is invalid</exception>
        public EasyEncrypt(SymmetricAlgorithm algorithm = null, byte[] key = null)
        {
            _algorithm = algorithm ?? Aes.Create();
            if (key == null) _algorithm.GenerateKey();
            else if (_algorithm.ValidKeySize(key.Length * 8)) _algorithm.Key = key;
            else throw new ArgumentException("Can't create EasyEncrypt: key is invalid");
        }

        /// <summary>
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt">random string to make key generation more random</param>
        /// <param name="algorithm">algorithm used for encryption, Aes if null</param>
        /// <param name="keysize">size of generated key, greatest key for algorithm if null</param>
        /// <exception cref="ArgumentException">can't create EasyEncrypt: key is invalid</exception>
        public EasyEncrypt(string password, string salt, SymmetricAlgorithm algorithm = null, int? keysize = null)
        {
            _algorithm = algorithm ?? Aes.Create();
            keysize ??= _algorithm.LegalKeySizes[0].MaxSize;
            _algorithm.Key = CreateKey(password, salt, (int) keysize);
        }

        /// <summary>
        /// Encrypt a string 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoder">encoding type (Default: UTF8)</param>
        /// <returns>IV + encrypted text</returns>
        public string Encrypt(string text, Encoding encoder = null)
            => Convert.ToBase64String(Encrypt(
                (encoder ?? Encoding.UTF8).GetBytes(
                    text ?? throw new ArgumentException("Can't encrypt text: text is null"))));

        /// <summary>
        /// Encrypt a byte[] 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>IV + encrypted data</returns>
        /// <exception cref="Exception">can't encrypt data: data is null</exception>
        public byte[] Encrypt(byte[] data)
        {
            if (data == null) throw new Exception("Can't encrypt data: data is null");

            using var ms = new MemoryStream();
            _algorithm.GenerateIV();
            ms.Write(_algorithm.IV, 0, _algorithm.IV.Length);

            using var cs = new CryptoStream(ms, _algorithm.CreateEncryptor(_algorithm.Key, _algorithm.IV),
                CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        /// <summary>
        /// Decrypt a string 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoder">encoding type (Default: UTF8)</param>
        /// <returns>IV + decrypted data</returns>
        public string Decrypt(string text, Encoding encoder = null)
            => (encoder ?? Encoding.UTF8).GetString(Decrypt(
                Convert.FromBase64String(
                    text ?? throw new ArgumentException("Can't decrypt data: text is null"))));

        /// <summary>
        /// Decrypt a byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <returns>IV + decrypted data</returns>
        /// <exception cref="ArgumentException">can't decrypt data: data is invalid</exception>
        public byte[] Decrypt(byte[] data)
        {
            if (data == null||data.Length <= 4) throw new ArgumentException("Can't decrypt data: data is invalid");

            byte[] iv = new byte[_algorithm.IV.Length];
            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
            _algorithm.IV = iv;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, _algorithm.CreateDecryptor(_algorithm.Key, _algorithm.IV),
                CryptoStreamMode.Write);
            cs.Write(data, iv.Length, data.Length - iv.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        /// <summary>
        /// Return the current key
        /// </summary>
        /// <returns>encryption key</returns>
        public byte[] GetKey() => _algorithm.Key;
        
        /// <summary>
        /// Dispose algorithm 
        /// </summary>
        public void Dispose()
        {
            _algorithm?.Dispose();
        }
        
        /// <summary>
        /// Generate new key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="keysize">keysize in bits</param>
        /// <returns>generated key</returns>
        /// <exception cref="ArgumentException">can't create key: {reason}</exception>
        public static byte[] CreateKey(string password, string salt, int keysize)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Can't create key: password is empty");
            if (salt == null || salt.Length < 8) throw new ArgumentException("Can't create key: salt is too short");
            if (keysize <= 0) throw new ArgumentException("Can't create key: keysize is invalid");

            using var rfc = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            return rfc.GetBytes(keysize / 8);
        }
    }
}

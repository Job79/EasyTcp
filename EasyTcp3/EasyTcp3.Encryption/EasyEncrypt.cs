using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasyTcp.Encryption
{
    public class EasyEncrypt
    {
        /// <summary>
        /// Algorithm that is used for encryption and decryption data.
        /// </summary> 
        private readonly SymmetricAlgorithm _algorithm;
        
        public EasyEncrypt(SymmetricAlgorithm algorithm = null, byte[] key = null)
        {
            _algorithm = algorithm ?? Aes.Create();
            if (key == null) _algorithm.GenerateKey();
            else if (_algorithm.ValidKeySize(key.Length * 8)) _algorithm.Key = key;
            else throw new ArgumentException("Can't create EasyEncrypt: key is invalid");
        }

        public EasyEncrypt(string password, string salt, SymmetricAlgorithm algorithm = null, int? keysize = null)
        {
            _algorithm = algorithm ?? Aes.Create();
            keysize ??= _algorithm.LegalKeySizes[0].MaxSize;
            _algorithm.Key = CreateKey(password, salt, (int) keysize);
        }

        public string Encrypt(string text, Encoding encoder = null)
            => Convert.ToBase64String(Encrypt(
                (encoder ?? Encoding.UTF8).GetBytes(
                    text ?? throw new ArgumentException("Can't encrypt text: text is null"))));

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

        public string Decrypt(string text, Encoding encoder = null)
            => (encoder ?? Encoding.UTF8).GetString(Decrypt(
                Convert.FromBase64String(
                    text ?? throw new ArgumentException("Can't decrypt data: text is null"))));

        public byte[] Decrypt(byte[] data)
        {
            if (data == null) throw new ArgumentException("Can't decrypt data: data is null");
            if (data.Length <= 4) throw new ArgumentException("Can't decrypt data: data is null");

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
        /// Return the current key.
        /// </summary>
        /// <returns>Encryption key</returns>
        public byte[] GetKey() => _algorithm.Key;

        /// <summary>
        /// Generate new key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="keysize">keysize in bits</param>
        /// <returns>generated key</returns>
        /// <exception cref="ArgumentException"></exception>
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
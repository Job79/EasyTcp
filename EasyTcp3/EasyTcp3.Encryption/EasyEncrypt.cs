using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasyTcp.Encryption
{
    public class EasyEncrypt
    {
        /// <summary>
        /// Algorithm for encryption and decryption data.
        /// </summary> 
        private readonly SymmetricAlgorithm _algorithm;

        public EasyEncrypt(byte[] key = null, SymmetricAlgorithm algorithm = null)
        {
            _algorithm = algorithm ?? Aes.Create();
            if (key == null) _algorithm.GenerateKey();
            else if (_algorithm.ValidKeySize(key.Length * 8)) _algorithm.Key = key;
            else throw new ArgumentException("Can't create EasyEncrypt: key is invalid");
        }

        public string Encrypt(string text, Encoding encoder = null)
            => Convert.ToBase64String(Encrypt(
                (encoder ?? Encoding.UTF8).GetBytes(
                    text ?? throw new ArgumentException("Can't encrypt text: text is null"))));

        public byte[] Encrypt(byte[] data)
        {
            if (data == null) throw new Exception("Can't encrypt data: data is null");

            _algorithm.GenerateIV(); //Generate new random IV.

            using var ms = new MemoryStream();
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
    }
}
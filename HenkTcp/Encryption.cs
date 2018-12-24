using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HenkTcp
{
    public static class Encryption
    {
        public static byte[] CreateKey(SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkEncryptSalt", int Iterations = 10000, int KeySize = 0)
        {
            if (Salt.Length < 8) throw new Exception("Salt is too short.");

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(Salt), Iterations);
            if (KeySize <= 0) { return key.GetBytes(Algorithm.Key.Length); }
            else { return key.GetBytes(KeySize); }
        }

        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, string Password, string Salt = "HenkEncryptSalt", int Iterations = 10000, int KeySize = 0) =>  Encrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key) => Convert.ToBase64String(Encrypt(Algorithm, Encoding.UTF8.GetBytes(Text), Key));
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password, string Salt = "HenkEncryptSalt", int Iterations = 10000, int KeySize = 0) => Encrypt(Algorithm, Data, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            Algorithm.Key = Key;
            Algorithm.GenerateIV();

            using (var ms = new MemoryStream())
            {
                ms.Write(Algorithm.IV, 0, Algorithm.IV.Length);
                using (var cs = new CryptoStream(ms, Algorithm.CreateEncryptor(Algorithm.Key, Algorithm.IV), CryptoStreamMode.Write))
                {
                    cs.Write(Data, 0, Data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, string Password, string Salt = "HenkEncryptSalt", int Iterations = 10000, int KeySize = 0) =>  Decrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize)); 
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key) => Encoding.UTF8.GetString(Decrypt(Algorithm, Convert.FromBase64String(Text), Key)); 
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password, string Salt = "HenkEncryptSalt", int Iterations = 10000, int KeySize = 0) => Decrypt(Algorithm, Data, CreateKey(Algorithm, Password, Salt, Iterations, KeySize)); 
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            Algorithm.Key = Key;
            using (var ms = new MemoryStream(Data))
            {
                byte[] iv = new byte[Algorithm.IV.Length];
                ms.Read(iv, 0, iv.Length);
                Algorithm.IV = iv;

                using (var cs = new CryptoStream(ms, Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV), CryptoStreamMode.Read))
                {
                    byte[] Decrypted = new byte[Data.Length];
                    int byteCount = cs.Read(Decrypted, 0, Data.Length);
                    return new MemoryStream(Decrypted, 0, byteCount).ToArray();
                }
            }
        }
    }
}
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace HenkTcp
{
    public static class Encryption
    {
        public static byte[] CreateKey(SymmetricAlgorithm Algorithm, string Password, int Iterations = 10000, int KeySize = 0, string Salt = "HenkTcpSalt")
        {
            if (Salt.Length < 8) { throw new Exception("Salt is too short"); }
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(Salt), Iterations);
            if (KeySize <= 0) { return key.GetBytes(Algorithm.Key.Length); }
            else { return key.GetBytes(KeySize); }
        }

        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, string Password) { return Encrypt(Algorithm, Text, CreateKey(Algorithm, Password)); }
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key) { return Convert.ToBase64String(Encrypt(Algorithm, Encoding.UTF8.GetBytes(Text), Key)); }
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password) { return Encrypt(Algorithm, Data, CreateKey(Algorithm, Password)); }
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            try
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
            catch { return null; }
        }

        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, string Password) { return Decrypt(Algorithm, Text, CreateKey(Algorithm, Password)); }
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key) { return Encoding.UTF8.GetString(Decrypt(Algorithm, Convert.FromBase64String(Text), Key)); }
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password) { return Decrypt(Algorithm, Data, CreateKey(Algorithm, Password)); }
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            try
            {
                Algorithm.Key = Key;
                using (var ms = new MemoryStream(Data))
                {
                    byte[] iv = new byte[Algorithm.IV.Length];
                    ms.Read(iv, 0, iv.Length);
                    Algorithm.IV = iv;

                    using (var cs = new CryptoStream(ms, Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV), CryptoStreamMode.Read))
                    {
                        byte[] decrypted = new byte[Data.Length];
                        var byteCount = cs.Read(decrypted, 0, Data.Length);
                        return new MemoryStream(decrypted, 0, byteCount).ToArray();
                    }
                }
            }
            catch { return null; }
        }
    }
}

/* HenkTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace HenkTcp
{
    public static class Encryption
    {
        /// <summary>
        /// Create a new key with PBKDF2.
        /// </summary>
        public static byte[] CreateKey(SymmetricAlgorithm Algorithm, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
        {
            if (Salt.Length < 8) throw new Exception("Salt is to short.");
            if (string.IsNullOrEmpty(Password)) throw new Exception("Password can't be empty.");
            if (Iterations <= 0) throw new Exception("Invalid Iterations.");
            if (KeySize == 0) throw new Exception("Invalid KeySize.");

            //Generate new key with PBKDF2
            Rfc2898DeriveBytes Key = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(Salt), Iterations);
            if (KeySize <= 0) { return Key.GetBytes(Algorithm.Key.Length); }//Use default key length for alghoritm
            else { return Key.GetBytes(KeySize); }//Use custom key length for alghoritm
        }

        /// <summary>
        /// Encrypt string with custom encoding.
        /// </summary>
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, string Password, Encoding Encoder, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Encrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize), Encoder);
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key, Encoding Encoder)
            => Convert.ToBase64String(Encrypt(Algorithm, Encoder.GetBytes(Text), Key));

        /// <summary>
        /// Encrypt string with UTF8.
        /// </summary>
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Encrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static string Encrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key)
            => Convert.ToBase64String(Encrypt(Algorithm, Encoding.UTF8.GetBytes(Text), Key));

        /// <summary>
        /// Encrypt byte[].
        /// </summary>
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Encrypt(Algorithm, Data, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static byte[] Encrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            if (Data == null) throw new Exception("Data can't be null.");
            else if (Key == null) throw new Exception("Key can't be null, did you start the server/connect the client with encryption enabled?");
            else if (Algorithm == null) throw new Exception("Algorithm can't be null, did you start the server/connect the client with encryption enabled?");

            Algorithm.Key = Key;//Set key.     
            Algorithm.GenerateIV();//Genarate new random IV.

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(Algorithm.IV, 0, Algorithm.IV.Length);//Write IV to ms(first 16 bytes)
                using (CryptoStream cs = new CryptoStream(ms, Algorithm.CreateEncryptor(Algorithm.Key, Algorithm.IV), CryptoStreamMode.Write))
                {
                    cs.Write(Data, 0, Data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypt string with UTF8.
        /// </summary>
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, string Password, Encoding Encoder, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Decrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key, Encoding Encoder)
            => Encoder.GetString(Decrypt(Algorithm, Convert.FromBase64String(Text), Key));

        /// <summary>
        /// Decrypt string with custom encoding.
        /// </summary>
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Decrypt(Algorithm, Text, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static string Decrypt(SymmetricAlgorithm Algorithm, string Text, byte[] Key)
            => Encoding.UTF8.GetString(Decrypt(Algorithm, Convert.FromBase64String(Text), Key));

        /// <summary>
        /// Decrypt byte[].
        /// </summary>
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, ushort KeySize = 0)
            => Decrypt(Algorithm, Data, CreateKey(Algorithm, Password, Salt, Iterations, KeySize));
        public static byte[] Decrypt(SymmetricAlgorithm Algorithm, byte[] Data, byte[] Key)
        {
            if (Data == null) throw new Exception("Data can't be null.");
            else if (Key == null) throw new Exception("Key can't be null, did you start the server/connect the client with encryption enabled?");
            else if (Algorithm == null) throw new Exception("Algorithm can't be null, did you start the server/connect the client with encryption enabled?");

            Algorithm.Key = Key;
            using (MemoryStream ms = new MemoryStream(Data))
            {
                byte[] IV = new byte[Algorithm.IV.Length];
                ms.Read(IV, 0, IV.Length);//Get IV from ms(first 16 bytes)               
                Algorithm.IV = IV;//Add IV to Algorithm

                using (CryptoStream cs = new CryptoStream(ms, Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV), CryptoStreamMode.Read))
                {
                    byte[] Decrypted = new byte[Data.Length];
                    int byteCount = cs.Read(Decrypted, 0, Data.Length);
                    return new MemoryStream(Decrypted, 0, byteCount).ToArray();
                }
            }
        }
    }
}
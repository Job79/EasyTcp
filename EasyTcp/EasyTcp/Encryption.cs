/* EasyTcp
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

namespace EasyTcp
{
    public class Encryption
    {
        /// <summary>
        /// Algorithm used for encryption/decryption.
        /// </summary>
        private SymmetricAlgorithm _Algorithm;

        /// <summary>
        /// Create class with an already set up algorithm.
        /// </summary>
        /// <param name="Algorithm">Algorithm wich is alreay set up properly</param>
        public Encryption(SymmetricAlgorithm Algorithm)
            => _Algorithm = Algorithm ?? throw new ArgumentException("Invalid algorithm, algorithm is null.");
        /// <summary>
        /// Create class with an algorithm and overide the key for the selected algorithm.
        /// </summary>
        /// <param name="Algorithm">New algorithm</param>
        /// <param name="Key">Generated key</param>
        public Encryption(SymmetricAlgorithm Algorithm, byte[] Key)
        {
            _Algorithm = Algorithm ?? throw new ArgumentException("Invalid algorithm, algorithm is null.");
            if (Key == null) throw new ArgumentException("Invalid key, key is null.");
            if (!_Algorithm.ValidKeySize(Key.Length * 8)) throw new ArgumentException("Invalid key, key has an invalid size.");
            _Algorithm.Key = Key;
        }
        /// <summary>
        /// Create class and create a new key for the passed algorithm.
        /// </summary>
        /// <param name="Algorithm">New algorithm</param>
        /// <param name="Password">Password, used to generate key</param>
        /// <param name="Salt">Salt, used to make generated key more random(min 8 characters)</param>
        /// <param name="Iterations">Rounds PBKDF2 will make to genarete a key</param>
        public Encryption(SymmetricAlgorithm Algorithm, string Password, string Salt, int Iterations = 10000)
        {
            _Algorithm = Algorithm ?? throw new ArgumentException("Invalid algorithm, algorithm is null.");
            _Algorithm.Key = CreateKey(_Algorithm, Password, Salt, Iterations);
        }
        /// <summary>
        /// Create class and create a new key for the passed algorithm with a fixed keysize.
        /// </summary>
        /// <param name="Algorithm">new algorithm</param>
        /// <param name="KeySize">Keysize in bits(8 bits = 1 byte)</param>
        /// <param name="Password">Password, used to generate key</param>
        /// <param name="Salt">Salt, used to make generated key more random(min 8 characters)</param>
        /// <param name="Iterations">Rounds PBKDF2 will make to genarete a key</param>
        public Encryption(SymmetricAlgorithm Algorithm, int KeySize, string Password, string Salt, int Iterations = 10000)
        {
            _Algorithm = Algorithm ?? throw new ArgumentException("Invalid algorithm, algorithm is null.");
            if (!_Algorithm.ValidKeySize(KeySize)) throw new ArgumentException("Invalid key, key has an invalid size.");
            Algorithm.Key = CreateKey(KeySize, Password, Salt, Iterations);
        }

        /// <summary>
        /// Encrypt a string and decode with UTF8.
        /// </summary>
        /// <param name="Text">Text to encrypt</param>
        /// <returns>Encrypted text(base64 string)</returns>
        public string Encrypt(string Text)
            => Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(Text ?? throw new ArgumentException("Could not decrypt text: Text can't be null."))));
        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="Text">Text to encrypt</param>
        /// <param name="Encoder">Encoding used to convert string to byte[]</param>
        /// <returns>Encrypted text(base64 string)</returns>
        public string Encrypt(string Text, Encoding Encoder)
            => Convert.ToBase64String(Encrypt(Encoder.GetBytes(Text ?? throw new ArgumentException("Could not decrypt text: Text can't be null."))));
        /// <summary>
        /// Encrypt a byte[].
        /// </summary>
        /// <param name="Data">Data to encrypt</param>
        /// <returns>Encrypted data(byte[])</returns>
        public byte[] Encrypt(byte[] Data)
        {
            if (Data == null) throw new Exception("Could not encrypt data: Data can't be null.");

            _Algorithm.GenerateIV();//Genarate new random IV.

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(_Algorithm.IV, 0, _Algorithm.IV.Length);//Write IV to ms(first 16 bytes)
                using (CryptoStream cs = new CryptoStream(ms, _Algorithm.CreateEncryptor(_Algorithm.Key, _Algorithm.IV), CryptoStreamMode.Write))
                {
                    cs.Write(Data, 0, Data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypt a string and decode with UTF8.
        /// </summary>
        /// <param name="Text">Text to decrypt</param>
        /// <returns>Decrypted text(string encoded with UTF8)</returns>
        public string Decrypt(string Text)
            => Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(Text ?? throw new ArgumentException("Could not decrypt text: Text can't be null."))));
        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="Text">Text to decrypt</param>
        /// <param name="Encoder">Encoding used to convert byte[] to string</param>
        /// <returns>Decrypted text(string encoded with Encoder)</returns>
        public string Decrypt(string Text, Encoding Encoder)
            => Encoder.GetString(Decrypt(Convert.FromBase64String(Text ?? throw new ArgumentException("Could not decrypt text: Text can't be null."))));
        /// <summary>
        /// Decrypt byte[].
        /// </summary>
        /// <param name="Data">Data to decrypt</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] Data)
        {
            if (Data == null) throw new ArgumentException("Could not encrypt data: Data can't be null.");

            using (MemoryStream ms = new MemoryStream(Data))
            {
                byte[] IV = new byte[_Algorithm.IV.Length];
                ms.Read(IV, 0, IV.Length);//Get IV from ms(first 16 bytes)
                _Algorithm.IV = IV;

                using (CryptoStream cs = new CryptoStream(ms, _Algorithm.CreateDecryptor(_Algorithm.Key, _Algorithm.IV), CryptoStreamMode.Read))
                {
                    byte[] Decrypted = new byte[Data.Length];
                    int byteCount = cs.Read(Decrypted, 0, Data.Length);
                    return new MemoryStream(Decrypted, 0, byteCount).ToArray();
                }
            }
        }

        /*static members*/
        /// <summary>
        /// Create a new encryption key with PBKDF2 and a selected keysize.
        /// </summary>
        /// <param name="KeySize">Keysize in bits(8 bits = 1 byte)</param>
        /// <param name="Password">Password, used to generate key</param>
        /// <param name="Salt">Salt, used to make generated key more random(min 8 characters)</param>
        /// <param name="Iterations">Rounds PBKDF2 will make to genarete a key</param>
        /// <returns>Encryption key</returns>
        public static byte[] CreateKey(int KeySize, string Password, string Salt, int Iterations = 10000)
        {
            if (KeySize <= 0) throw new ArgumentException("Could not create key: Invalid KeySize.");
            else if (Salt.Length < 8) throw new ArgumentException("Could not create key: Salt is to short.");
            else if (string.IsNullOrEmpty(Password)) throw new ArgumentException("Could not create key: Password can't be empty.");
            else if (Iterations <= 0) throw new ArgumentException("Could not create key: Invalid Iterations count.");

            return new Rfc2898DeriveBytes(Password,
                Encoding.UTF8.GetBytes(Salt),//Convert salt to byte[]
                Iterations).GetBytes(KeySize / 8);
        }
        /// <summary>
        /// Create a new encryption key with PBKDF2 for the selected Algorithm.
        /// </summary>
        /// <param name="Algorithm">Algorithm is used to get the keysize</param>
        /// <param name="Password">Password, used to generate key</param>
        /// <param name="Salt">Salt, used to make generated key more random(min 8 characters)</param>
        /// <param name="Iterations">Rounds PBKDF2 will make to genarete a key</param>
        /// <returns>Encryption key</returns>
        public static byte[] CreateKey(SymmetricAlgorithm Algorithm, string Password, string Salt, int Iterations = 10000)
            => CreateKey(Algorithm.LegalKeySizes[0].MaxSize, Password, Salt, Iterations);
    }
}

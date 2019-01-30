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
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace HenkTcp
{
    public class Message
    {
        /// <summary>
        /// Get the sender as TcpClient.
        /// </summary>
        public readonly TcpClient TcpClient;

        /// <summary>
        /// Variables that are used for the encryption.
        /// </summary>
        private readonly byte[] _EncryptionKey;
        private readonly SymmetricAlgorithm _Algorithm;

        /// <summary>
        /// Encoding will be used to encode string's.
        /// </summary>
        private readonly Encoding _Encoding;

        public Message(byte[] Data, TcpClient TcpClient, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, Encoding Encoding)
        {
            this.Data = Data;
            this.TcpClient = TcpClient;

            _Encoding = Encoding;
            _EncryptionKey = EncryptionKey;
            _Algorithm = Algorithm;
        }

        /// <summary>
        /// Convert and return the data as Short(Int16).
        /// </summary>
        public short GetShort { get { return BitConverter.ToInt16(Data,0); } }
        public short GetShortDecrypted { get { return BitConverter.ToInt16(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as int(Int32).
        /// </summary>
        public int GetInt { get { return BitConverter.ToInt32(Data, 0); } }
        public int GetIntDecrypted { get { return BitConverter.ToInt32(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as long(Int64).
        /// </summary>
        public long GetLong { get { return BitConverter.ToInt64(Data, 0); } }
        public long GetLongDecrypted { get { return BitConverter.ToInt64(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as double(Double).
        /// </summary>
        public double GetDouble { get { return BitConverter.ToDouble(Data, 0); } }
        public double GetDoubleDecrypted { get { return BitConverter.ToDouble(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as float(Single).
        /// </summary>
        public float GetFloat { get { return BitConverter.ToSingle(Data, 0); } }
        public float GetFloatDecrypted { get { return BitConverter.ToSingle(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as bool(Boolean).
        /// </summary>
        public bool GetBool { get { return BitConverter.ToBoolean(Data, 0); } }
        public bool GetBoolDecrypted { get { return BitConverter.ToBoolean(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as char(Char).
        /// </summary>
        public char GetChar { get { return BitConverter.ToChar(Data, 0); } }
        public char GetCharDecrypted { get { return BitConverter.ToChar(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as object.
        /// This will use the HenkTcp/Serialization class.
        /// </summary>
        public T GetObject<T>() where T : class => Serialization.Deserialize<T>(Data);
        public T GetObjectDecrypted<T>() where T : class => Serialization.Deserialize<T>(DataDecrypted);

        /// <summary>
        /// Convert and return the data as string.
        /// The selected encoding will be used. (Default UTF8)
        /// </summary>
        public string GetString { get { return _Encoding.GetString(Data); } }
        public string GetStringDecrypted { get { return _Encoding.GetString(DataDecrypted); } }

        /// <summary>
        /// Return the length of the received bytes.
        /// </summary>
        public int BytesLength { get { return Data.Length; } }

        /// <summary>
        /// Return the received bytes.
        /// </summary>
        public readonly byte[] Data;
        public byte[] DataDecrypted
        {
            get
            {
                if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
                return Encryption.Decrypt(_Algorithm, Data, _EncryptionKey);
            }
        }

        /// <summary>
        /// Send data to the sender.
        /// </summary>
        public void Reply(short Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(int Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(long Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(double Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(float Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(bool Data) => Reply(BitConverter.GetBytes(Data));
        public void Reply(char Data) => Reply(BitConverter.GetBytes(Data));  
        public void Reply(object Data) => Reply(Serialization.Serialize(Data));
        public void Reply(string Data) => Reply(_Encoding.GetBytes(Data));
        public void Reply(byte[] Data) =>  TcpClient.GetStream().Write(Data, 0, Data.Length);

        /// <summary>
        /// Encrypt data end send data to sender.
        /// </summary>
        public void ReplyEncrypted(short Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(int Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(long Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(double Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(float Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(bool Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(char Data) => ReplyEncrypted(BitConverter.GetBytes(Data));
        public void ReplyEncrypted(object Data) => ReplyEncrypted(Serialization.Serialize(Data));
        public void ReplyEncrypted(string Data) => ReplyEncrypted(_Encoding.GetBytes(Data));
        public void ReplyEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
            Reply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        /// <summary>
        /// Return the IP of the sender.
        /// </summary>
        public string ClientIP { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }
    }
}

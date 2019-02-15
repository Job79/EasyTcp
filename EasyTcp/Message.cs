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
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace EasyTcp
{
    public class Message
    {
        /// <summary>
        /// Server: Return the sender as Socket.
        /// Client: Return the client Socket.
        /// </summary>
        public readonly Socket Socket;

        /// <summary>
        /// Encoding to encode string's
        /// </summary>
        public readonly Encoding Encoding;

        /// <summary>
        /// Encryption class for encrypting/decrypting data.
        /// </summary>
        private readonly Encryption Encryption;

        public Message(byte[] Data, Socket Socket, Encryption Encryption, Encoding Encoding)
        {
            this.Data = Data;
            this.Socket = Socket;
            this.Encryption = Encryption;
            this.Encoding = Encoding;
        }

        /// <summary>
        /// Return the received byte's
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// Return the received byte's
        /// </summary>
        public byte[] DataDecrypted { get { return (Encryption ?? throw new NullReferenceException("Could not decrypt data: Encryption class is null.")).Decrypt(Data); } }

        /// <summary>
        /// Convert and return the data as short(Int16).
        /// </summary>
        public short GetShort { get { return BitConverter.ToInt16(Data, 0); } }
        /// <summary>
        /// Convert and return the data as short(Int16).
        /// </summary>
        public short GetShortDecrypted { get { return BitConverter.ToInt16(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as int(Int32).
        /// </summary>
        public int GetInt { get { return BitConverter.ToInt32(Data, 0); } }
        /// <summary>
        /// Convert and return the data as int(Int32).
        /// </summary>
        public int GetIntDecrypted { get { return BitConverter.ToInt32(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as long(Int64).
        /// </summary>
        public long GetLong { get { return BitConverter.ToInt64(Data, 0); } }
        /// <summary>
        /// Convert and return the data as long(Int64).
        /// </summary>
        public long GetLongDecrypted { get { return BitConverter.ToInt64(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as double(Double).
        /// </summary>
        public double GetDouble { get { return BitConverter.ToDouble(Data, 0); } }
        /// <summary>
        /// Convert and return the data as double(Double).
        /// </summary>
        public double GetDoubleDecrypted { get { return BitConverter.ToDouble(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as float(Single).
        /// </summary>
        public float GetFloat { get { return BitConverter.ToSingle(Data, 0); } }
        /// <summary>
        /// Convert and return the data as float(Single).
        /// </summary>
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
        /// <summary>
        /// Convert and return the data as char(Char).
        /// </summary>
        public char GetCharDecrypted { get { return BitConverter.ToChar(DataDecrypted, 0); } }

        /// <summary>
        /// Convert and return the data as string.
        /// </summary>
        public string GetString { get { return Encoding.GetString(Data); } }
        /// <summary>
        /// Convert and return the data as string.
        /// </summary>
        public string GetStringDecrypted { get { return Encoding.GetString(DataDecrypted); } }

        /// <summary>
        /// Encrypt data and send data(short) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(short Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        ///Encrypt data and send data(int) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(int Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(long) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(long Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(double) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(double Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(float) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(float Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(bool) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(bool Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(char) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(char Data)
            => ReplyEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data and send data(string) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(string Data)
            => ReplyEncrypted(Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data and send data(byte[]) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void ReplyEncrypted(byte[] Data)
            => Reply((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(short Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(int) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(int Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(long) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(long Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(double) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(double Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(float) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(float Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(bool) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(bool Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(char) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(char Data) => Reply(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(string) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(string Data) => Reply(Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to the sender.
        /// </summary>
        /// <param name="Data">Data that will be send to sender</param>
        public void Reply(byte[] Data)
        {
            if (Data == null) throw new Exception("Could not send data: Data is empty.");

            byte[] Message = new byte[Data.Length + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, Message, 0, 4);
            Buffer.BlockCopy(Data, 0, Message, 4, Data.Length);

            Socket.SendAsync(Message, SocketFlags.None);
        }

        /// <summary>
        /// Return the IP of the Socket.
        /// </summary>
        public string ClientIP { get { return ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString(); } }
    }
}

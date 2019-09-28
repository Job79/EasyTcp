/* EasyTcp
 * 
 * Copyright (c) 2019 henkje
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
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
        private readonly Encryption encryption;

        public Message(byte[] data, Socket socket, Encryption encryption, Encoding encoding)
        {
            Data = data;
            Socket = socket;
            this.encryption = encryption;
            Encoding = encoding;
        }

        /// <summary>
        /// Return the received byte's
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// Return the received byte's
        /// </summary>
        public byte[] DataDecrypted { get { return (encryption ?? throw new NullReferenceException("Could not decrypt data: Encryption class is null.")).Decrypt(Data); } }

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
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(short data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        ///Encrypt data and send data(int) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(int data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(long) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(long data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(double) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(double data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(float) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(float data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(bool) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(bool data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(char) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(char data)
            => ReplyEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data and send data(string) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(string data)
            => ReplyEncrypted(Encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data and send data(byte[]) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void ReplyEncrypted(byte[] data)
            => Reply((encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(short data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(int) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(int data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(long) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(long data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(double) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(double data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(float) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(float data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(bool) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(bool data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(char) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(char data) => Reply(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(string) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(string data) => Reply(Encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to the sender.
        /// </summary>
        /// <param name="data">Data that will be send to sender</param>
        public void Reply(byte[] data)
        {
            if (data == null) throw new Exception("Could not send data: Data is empty.");

            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);

            using (SocketAsyncEventArgs e = new SocketAsyncEventArgs())
            {
                e.SetBuffer(message, 0, message.Length);
                Socket.SendAsync(e);//Write async so it won't block UI applications.
            }
        }

        /// <summary>
        /// Return the IP of the Socket.
        /// </summary>
        public string ClientIP { get { return ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString(); } }
    }
}

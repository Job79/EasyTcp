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
using System.Threading;
using System.Net.Sockets;

namespace EasyTcp.Client
{
    public class EasyTcpClient
    {
        public Socket Socket { get; private set; }

        /// <summary>
        /// DataReceived, triggerd when a message is received and no other data is avaible.
        /// </summary>
        public event EventHandler<Message> DataReceived;
        /// <summary>
        /// OnDisconect, triggerd when a client disconnect's from the server.
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;
        /// <summary>
        /// OnError, triggerd when an error occurs.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Encoding to encode string's
        /// </summary>
        private Encoding encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value ?? throw new ArgumentNullException("Encoding can't be set to null."); }
        }

        /// <summary>
        /// Encryption class for encrypting/decrypting data.
        /// </summary>
        public Encryption Encryption;

        /// <summary>
        /// Max bytes the client can receive in 1 message.
        /// </summary>
        private ushort maxDataSize;

        /// <summary>
        /// Data buffer for incoming data.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Convert string to IPAddress.
        /// Used by the Connect overloads.
        /// </summary>
        /// <param name="IPString">IP(IPv4 or IPv6) as string</param>
        /// <returns>IP as IPAddress</returns>
        private IPAddress GetIP(string IPString)
        {
            if (!IPAddress.TryParse(IPString, out IPAddress IP))
                throw new ArgumentException("Invalid IPv4/IPv6 address.");
            return IP;
        }

        /// <summary>
        /// Connect to server and set encryption.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="port">Port as ushort(0-65 535)</param>
        /// <param name="timeout">Time it maximum can take to connect to server</param>
        /// <param name="encryption">Encryption will set <see cref="EasyTcp.Encryption"/></param>
        /// <param name="maxDataSize">Max size of a message client can receive</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>
        public bool Connect(string IP, ushort port, TimeSpan timeout, Encryption encryption, ushort maxDataSize = 1024)
        {
            Encryption = encryption;
            return Connect(GetIP(IP), port, timeout, maxDataSize);
        }
        /// <summary>
        /// Connect to server and set encryption.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="port">Port as ushort(0-65 535)</param>
        /// <param name="timeout">Time it maximum can take to connect to server</param>
        /// <param name="encryption">Encryption will set <see cref="EasyTcp.Encryption"/></param>
        /// <param name="maxDataSize">Max size of a message client can receive</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>
        public bool Connect(IPAddress IP, ushort port, TimeSpan timeout, Encryption encryption, ushort maxDataSize = 1024)
        {
            Encryption = encryption;
            return Connect(IP, port, timeout, maxDataSize);
        }
        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="Port">Port as ushort(0-65 535)</param>
        /// <param name="Timeout">Time it maximum can take to connect to server</param>
        /// <param name="MaxDataSize">Max size of a message client can receive</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>
        public bool Connect(string IP, ushort port, TimeSpan timeout, ushort maxDataSize = 1024)
            => Connect(GetIP(IP), port, timeout, maxDataSize);
        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="port">Port as ushort(0-65 535)</param>
        /// <param name="timeout">Time it maximum can take to connect to server</param>
        /// <param name="maxDataSize">Max size of a message client can receive</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>
        public bool Connect(IPAddress IP, ushort port, TimeSpan timeout, ushort maxDataSize = 1024)
        {
            if (IP == null) throw new ArgumentNullException("Could not connect: Invalid IP.");
            else if (port == 0) throw new ArgumentException("Could not connect: Invalid Port.");
            else if (timeout.Ticks.Equals(0)) throw new ArgumentException("Could not connect: Invalid Timeout.");
            else if (maxDataSize <= 0) throw new ArgumentException("Could not connect: Invalid MaxDataSize.");

            //Create socket.
            Socket = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Try to connect.
            try { Socket.BeginConnect(IP, port,null,null).AsyncWaitHandle.WaitOne(timeout); }
            catch { Socket = null; return false; }

            //Check if socket is connected or timout exired.
            if (Socket.Connected)
            {
                this.maxDataSize = maxDataSize;
                buffer = new byte[2];

                //Start listerning for data.
                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceiveLength, Socket);
                return true;
            }
            else { Socket = null; return false; }
        }

        /// <summary>
        /// Disconnect from server.
        /// </summary>
        /// <param name="notifyOnDisconnect">Calls the OnDisconnect handler if set to true</param>
        public void Disconnect(bool notifyOnDisconnect = false)
        {
            if (Socket == null) return;//Client is not connected.

            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket = null;
            }
            catch (Exception ex) { NotifyOnError(ex); }

            if (notifyOnDisconnect) OnDisconnect?.Invoke(this, this);//Call OnDisconnect.
        }

        /// <summary>
        /// Return the connection state of the connection.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Socket == null) return false;
                else if (Socket.Poll(0, SelectMode.SelectRead) && Socket.Available.Equals(0)) { Disconnect(true); return false; }
                else return true;
            }
        }

        #region Send
        /// <summary>
        /// Encrypt data(short) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(short data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(int) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(int data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(long) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(long data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(double) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(double data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(float) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(float data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(bool) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(bool data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(char) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(char data)
            => SendEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(string) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(string data)
            => SendEncrypted(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send data to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void SendEncrypted(byte[] data)
            => Send((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(short data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(int) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(int data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(long) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(long data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(double) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(double data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(float) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(float data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(bool) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(bool data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(char) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(char data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(string) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(string data)
            => Send(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (Socket == null) throw new Exception("Could not send data: Socket not connected.");

            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);

            using (SocketAsyncEventArgs e = new SocketAsyncEventArgs())
            {
                e.SetBuffer(message, 0, message.Length);
                Socket.SendAsync(e);//Write async so it won't block UI applications.
            }
        }
        #endregion

        #region SendAndGetReplyEncrypted
        /// <summary>
        /// Encrypt data(short) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(short data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(int) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(int data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(long) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(long data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(double) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(double data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(float) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(float data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(bool) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(bool data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(char) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(char data, TimeSpan timeout) 
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrypt data(string) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(string data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);
        /// <summary>
        /// Encrypt data(byte[]) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(byte[] data, TimeSpan timeout)
            => SendAndGetReply((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);

        /// <summary>
        /// Send data(short) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(short data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(int) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(int data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(long) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(long data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(double) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(double data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(float) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(float data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(bool) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(bool data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(char) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(char data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(string) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(string data, TimeSpan timeout)
            => SendAndGetReply(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);
        /// <summary>
        /// Send data(byte[]) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(byte[] data, TimeSpan timeout)
        {
            if (timeout.Ticks.Equals(0)) throw new ArgumentException("Invalid Timeout.");

            Message reply = null;
            using (ManualResetEventSlim signal = new ManualResetEventSlim())
            {
                void Event(object sender, Message e) { reply = e; DataReceived -= Event; signal.Set(); };

                DataReceived += Event;
                Send(data);

                signal.Wait(timeout);
                return reply;
            }
        }
        #endregion


        private void OnReceiveLength(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;

            try
            {
                //Test if client is connected.
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available.Equals(0))
                { Disconnect(true); return; }

                ushort DataLength = BitConverter.ToUInt16(buffer, 0);//Get the length of the data.

                if (DataLength <= 0 || DataLength > maxDataSize) { Disconnect(true); return; }//Invalid length, close connection.
                else socket.BeginReceive(buffer = new byte[DataLength], 0, DataLength, SocketFlags.None, OnReceiveData, socket);//Start accepting the data.
            }
            catch (SocketException) { Disconnect(false); OnDisconnect?.Invoke(this, this); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;

            try
            {
                //Test if client is connected.
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available.Equals(0))
                { Disconnect(true); return; }

                DataReceived?.Invoke(this, new Message(buffer, socket, Encryption, encoding));//Trigger event
                socket.BeginReceive(buffer = new byte[2], 0, buffer.Length, SocketFlags.None, OnReceiveLength, socket);//Start receiving next message.
            }
            catch (SocketException) { Disconnect(true); return; }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        /*This function is used to handle errors*/
        private void NotifyOnError(Exception ex)
        {
            if (OnError != null)
                OnError(this, ex); else throw ex;
        }
    }
}

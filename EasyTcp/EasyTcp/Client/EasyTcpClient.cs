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
using System.Threading;
using System.Net.Sockets;

namespace EasyTcp.Client
{
    public class EasyTcpClient
    {
        /// <summary>
        /// ClientSocket.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// DataReceived will be triggerd when a message is received and no other data is avaible.
        /// </summary>
        public event EventHandler<Message> DataReceived;
        /// <summary>
        /// OnDisconect will be triggerd when the client disconnect's from the server.
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;
        /// <summary>
        /// OnError will be triggerd when an error occurs.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Encoding to encode string's
        /// </summary>
        private Encoding _Encoding = Encoding.UTF8;
        public Encoding Encoding { get { return _Encoding; } set { _Encoding = value ?? throw new ArgumentNullException("You can't set Encoding to null."); } }

        /// <summary>
        /// Encryption class for encrypting/decrypting data.
        /// </summary>
        public Encryption Encryption;

        /// <summary>
        /// Max bytes the client can receive in 1 message.
        /// </summary>
        private int _MaxDataSize;

        /// <summary>
        /// Data buffer for incoming data.
        /// </summary>
        private byte[] _Buffer;

        /// <summary>
        /// Convert string to IPAddress.
        /// Used by the Connect overloads.
        /// </summary>
        /// <param name="IPString">IP(IPv4 or IPv6) as string</param>
        /// <returns>IP as IPAddress</returns>
        private IPAddress _GetIP(string IPString)
        {
            IPAddress IP;
            if (!IPAddress.TryParse(IPString, out IP)) throw new ArgumentException("Invalid IPv4/IPv6 address.");
            return IP;
        }

        /// <summary>
        /// Connect to server and set encryption.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="Port">Port as ushort(0-65 535)</param>
        /// <param name="Timeout">Time it maximum can take to connect to server</param>
        /// <param name="Encryption">Encryption will set <see cref="Encryption"/></param>
        /// <param name="MaxDataSize">Max bytes the client can receive in 1 message</param>
        /// <returns>bool, true = Connected, false = can not connect</returns>
        public bool Connect(string IP, ushort Port, TimeSpan Timeout, Encryption Encryption, int MaxDataSize = 10240)
        {
            this.Encryption = Encryption;
            return Connect(_GetIP(IP), Port, Timeout, MaxDataSize);
        }
        /// <summary>
        /// Connect to server and set encryption.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="Port">Port as ushort(0-65 535)</param>
        /// <param name="Timeout">Time it maximum can take to connect to server</param>
        /// <param name="Encryption">Encryption will set <see cref="Encryption"/></param>
        /// <param name="MaxDataSize">Max bytes the client can receive in 1 message</param>
        /// <returns>bool, true = Connected, false = can not connect</returns>
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, Encryption Encryption, int MaxDataSize = 10240)
        {
            this.Encryption = Encryption;
            return Connect(IP, Port, Timeout, MaxDataSize);
        }
        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="Port">Port as ushort(0-65 535)</param>
        /// <param name="Timeout">Time it maximum can take to connect to server</param>
        /// <param name="MaxDataSize">Max bytes the client can receive in 1 message</param>
        /// <returns>bool, true = Connected, false = can not connect</returns>
        public bool Connect(string IP, ushort Port, TimeSpan Timeout, int MaxDataSize = 10240)
            => Connect(_GetIP(IP), Port, Timeout, MaxDataSize);
        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="Port">Port as ushort(0-65 535)</param>
        /// <param name="Timeout">Time it maximum can take to connect to server</param>
        /// <param name="MaxDataSize">Max bytes the client can receive in 1 message</param>
        /// <returns>bool, true = Connected, false = can not connect</returns>
        public bool Connect(IPAddress IP, ushort Port, TimeSpan Timeout, int MaxDataSize = 10240)
        {
            if (IP == null) throw new ArgumentNullException("Could not connect: Invalid IP.");
            else if (Port == 0) throw new ArgumentException("Could not connect: Invalid Port.");
            else if (Timeout.Ticks.Equals(0)) throw new ArgumentException("Could not connect: Invalid Timeout.");
            else if (MaxDataSize <= 0) throw new ArgumentException("Could not connect: Invalid MaxDataSize.");

            //Create socket.
            Socket = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Try to connect.
            try { Socket.ConnectAsync(IP, Port).Wait(Timeout); }
            catch { Socket = null; return false; }

            //Check if socket is connected or timout exired.
            if (Socket.Connected)
            {
                _MaxDataSize = MaxDataSize;
                _Buffer = new byte[4];

                //Start listerning for data.
                Socket.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, _ReceiveLength, Socket);
                return true;
            }
            else { Socket = null; return false; }
        }

        /// <summary>
        /// Disconnect from server.
        /// </summary>
        /// <param name="NotifyOnDisconnect">Calls the OnDisconnect handler if set to true</param>
        public void Disconnect(bool NotifyOnDisconnect = false)
        {
            if (Socket == null) return;//Client is not connected.

            try { Socket.Shutdown(SocketShutdown.Both);  Socket = null; }
            catch (Exception ex) { _NotifyOnError(ex); }

            if (NotifyOnDisconnect) OnDisconnect?.Invoke(this, this);//Call OnDisconnect.
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
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(short Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(int) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(int Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(long) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(long Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(double) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(double Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(float) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(float Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(bool) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(bool Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(char) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(char Data)
            => SendEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(string) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(string Data)
            => SendEncrypted(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send data to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void SendEncrypted(byte[] Data)
            => Send((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(short Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(int) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(int Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(long) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(long Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(double) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(double Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(float) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(float Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(bool) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(bool Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(char) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(char Data)
            => Send(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(string) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(string Data)
            => Send(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        public void Send(byte[] Data)
        {
            if (Data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (Socket == null) throw new Exception("Could not send data: Socket not connected.");

                byte[] Message = new byte[Data.Length + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, Message, 0, 4);
                Buffer.BlockCopy(Data, 0, Message, 4, Data.Length);

                Socket.SendAsync(Message, SocketFlags.None);//Write async so it won't block UI applications.
        }
        #endregion

        #region SendAndGetReplyEncrypted
        /// <summary>
        /// Encrypt data(short) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(short Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(int) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(int Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(long) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(long Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(double) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(double Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(float) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(float Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(bool) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(bool Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(char) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(char Data, TimeSpan Timeout) => SendAndGetReplyEncrypted(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrypt data(string) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(string Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);
        /// <summary>
        /// Encrypt data(byte[]) and send data to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReplyEncrypted(byte[] Data, TimeSpan Timeout)
            => SendAndGetReply((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);

        /// <summary>
        /// Send data(short) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(short Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(int) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(int Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(long) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(long Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(double) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(double Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(float) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(float Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(bool) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(bool Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(char) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(char Data, TimeSpan Timeout)
            => SendAndGetReply(BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(string) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(string Data, TimeSpan Timeout)
            => SendAndGetReply(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);
        /// <summary>
        /// Send data(byte[]) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="Data">Data to send to server</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(byte[] Data, TimeSpan Timeout)
        {
            if (Timeout.Ticks.Equals(0)) throw new ArgumentException("Invalid Timeout.");

            Message Reply = null;
            ManualResetEventSlim Signal = new ManualResetEventSlim();

            void Event(object sender, Message e) { Reply = e; DataReceived -= Event; Signal.Set(); };

            DataReceived += Event;
            Send(Data);

            Signal.Wait(Timeout);
            return Reply;
        }
        #endregion


        private void _ReceiveLength(IAsyncResult ar)
        {
            Socket Socket = ar.AsyncState as Socket;

            try
            {
                //Test if client is connected.
                if (Socket.Poll(0, SelectMode.SelectRead) && Socket.Available.Equals(0))
                { Disconnect(true); return; }

                int DataLength = BitConverter.ToInt32(_Buffer, 0);//Get the length of the data.

                if (DataLength <= 0||DataLength > _MaxDataSize) { Disconnect(true); return; }//Invalid length, close connection.
                else Socket.BeginReceive(_Buffer = new byte[DataLength], 0, DataLength, SocketFlags.None, _ReceiveData, Socket);//Start accepting the data.
            }
            catch (SocketException) { Disconnect(false); OnDisconnect?.Invoke(this, this); }
            catch (Exception ex) { _NotifyOnError(ex); }
        }

        private void _ReceiveData(IAsyncResult ar)
        {
            Socket Socket = ar.AsyncState as Socket;

            try
            {
                //Test if client is connected.
                if (Socket.Poll(0, SelectMode.SelectRead) && Socket.Available.Equals(0))
                { Disconnect(true); return; }

                DataReceived?.Invoke(this, new Message(_Buffer, Socket, Encryption, _Encoding));//Trigger event
                Socket.BeginReceive(_Buffer = new byte[4], 0, _Buffer.Length, SocketFlags.None, _ReceiveLength, Socket);//Start receiving next length.
            }
            catch (SocketException) { Disconnect(true); return; }
            catch (Exception ex) { _NotifyOnError(ex); }
        }

        /*This function is used to handle errors*/
        private void _NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}

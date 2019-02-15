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
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace EasyTcp.Server
{
    public class EasyTcpServer
    {
        /* The ServerListener class controls all the event's.
         * ServerListener is set to null if the server is not running.*/
        private ServerListener _ServerListener;

        /// <summary>
        /// ClientConnected, triggerd when a new client connect's.
        /// </summary>
        public event EventHandler<Socket> ClientConnected;
        /// <summary>
        /// ClientDisconnected, triggerd when a client disconnect's.
        /// </summary>
        public event EventHandler<Socket> ClientDisconnected;
        /// <summary>
        /// DataReceived, triggerd when new data is received.
        /// </summary>
        public event EventHandler<Message> DataReceived;
        /// <summary>
        /// OnError, triggerd when an error occurs.
        /// </summary>
        public event EventHandler<Exception> OnError;
        /// <summary>
        /// ClientRefused, triggerd when a client is refused.
        /// Client will be refused when banned or when there are to many clients connected.
        /// </summary>
        public event EventHandler<RefusedClient> ClientRefused;

        /// <summary>
        /// Encoding to encode string's
        /// </summary>
        private Encoding _Encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value ?? throw new ArgumentNullException("Encoding can't be set to null."); }
        }

        /// <summary>
        /// Encryption class for encrypting/decrypting data.
        /// </summary>
        public Encryption Encryption;

        /// <summary>
        /// BannedIPs, refuse all ip's in this list.
        /// </summary>
        public HashSet<string> BannedIPs = new HashSet<string>();

        /// <summary>
        /// Convert string to IPAddress.
        /// Used by the Start overloads.
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
        /// Start the server and overide encryption.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="Port">Port as ushort(0-65,535)</param>
        /// <param name="MaxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="Encryption">Encryption class <see cref="EasyTcp.Encryption"/></param>
        /// <param name="DualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="MaxDataSize">Max size of a message the server can receive</param>
        public void Start(string IP, ushort Port, int MaxConnections, Encryption Encryption, bool DualMode = false, int MaxDataSize = 10240)
        {
            this.Encryption = Encryption;
            Start(_GetIP(IP), Port, MaxConnections, DualMode, MaxDataSize);
        }
        /// <summary>
        /// Start the server and overide encryption.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="Port">Port as ushort(0-65,535)</param>
        /// <param name="MaxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="Encryption">Encryption class <see cref="EasyTcp.Encryption"/></param>
        /// <param name="DualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="MaxDataSize">Max size of a message the server can receive</param>
        public void Start(IPAddress IP, ushort Port, int MaxConnections, Encryption Encryption, bool DualMode = false, int MaxDataSize = 10240)
        {
            this.Encryption = Encryption;
            Start(IP, Port, MaxConnections, DualMode, MaxDataSize);
        }
        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="Port">Port as ushort(0-65,535)</param>
        /// <param name="MaxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="DualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="MaxDataSize">Max size of a message the server can receive</param>
        public void Start(string IP, ushort Port, int MaxConnections, bool DualMode = false, int MaxDataSize = 10240)
            => Start(_GetIP(IP), Port, MaxConnections, DualMode, MaxDataSize);
        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="Port">Port as ushort(0-65,535)</param>
        /// <param name="MaxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="DualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="MaxDataSize">Max size of a message the server can receive</param>
        public void Start(IPAddress IP, ushort Port, int MaxConnections, bool DualMode = false, int MaxDataSize = 10240)
        {
            if (IsRunning) throw new Exception("Could not start server: Server is already running.");
            else if (IP == null) throw new ArgumentNullException("Could not start server: Ip is null");
            else if (Port == 0) throw new ArgumentException("Could not start server: Invalid Port.");
            else if (MaxConnections <= 0) throw new ArgumentException("Could not start server: Invalid MaxConnections count.");
            else if (MaxDataSize <= 0) throw new ArgumentException("Could not start server: Invalid MaxDataSize.");

            Socket Listener = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (IP.AddressFamily == AddressFamily.InterNetworkV6) Listener.DualMode = DualMode;
            Listener.Bind(new IPEndPoint(IP, Port));

            //Create class ServerListener, this will start the passed socket and handle the events.
            _ServerListener = new ServerListener(Listener, this, MaxConnections, MaxDataSize);
        }

        /// <summary>
        /// Return the ConnectedClients.
        /// </summary>
        public IEnumerable<Socket> ConnectedClients
        {
            get
            {
                if (!IsRunning) return null;
                else return _ServerListener.ConnectedClients.ToList();//.ToList because ConnectedClients is used by async funtions
            }
        }
        /// <summary>
        /// Return the count of ConnectedClients.
        /// </summary>
        public int ConnectedClientsCount
        {
            get
            {
                if (!IsRunning) return 0;
                else return _ServerListener.ConnectedClients.Count;
            }
        }

        /// <summary>
        /// Return the listener socket.
        /// </summary>
        public Socket Listener
        {
            get
            {
                if (!IsRunning) return null;
                else return _ServerListener.Listener;
            }
        }

        /// <summary>
        /// Return the state of the server.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _ServerListener != null;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                _ServerListener.IsListerning = false;
                _ServerListener.Listener.Close();
                _ServerListener = null;
            }
        }

        /// <summary>
        /// Kick a Socket.
        /// </summary>
        public void Kick(Socket Client) 
            => (Client ?? throw new ArgumentNullException("Could not kick client: Socket is null")).Shutdown(SocketShutdown.Both);

        /// <summary>
        /// Add the IP of the socket to BannedIPs and kick the client.
        /// </summary>
        public void Ban(Socket Client)
        {
            BannedIPs.Add(((IPEndPoint)(Client ?? throw new ArgumentNullException("Could not ban client: Socket is null")).RemoteEndPoint).Address.ToString());//Add client IP to banned IPs
            Kick(Client);//Kick client
        }

        #region Broadcast
        /// <summary>
        /// Encrypt data(short) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(short Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(int) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(int Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(long) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(long Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(double) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(double Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(float) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(float Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(bool) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(bool Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(char) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(char Data)
            => BroadcastEncrypted(BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(string) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(string Data)
            => BroadcastEncrypted(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(byte[] Data)
            => Broadcast((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(short Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(int) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(int Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(long) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(long Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(double) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(double Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(float) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(float Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(bool) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(bool Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(char) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(char Data)
            => Broadcast(BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(string) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(string Data)
            => Broadcast(_Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to all connected clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void Broadcast(byte[] Data)
        {
            if (Data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (!IsRunning)
            { NotifyOnError(new Exception("Could not send data: Server is not running.")); return; }

            byte[] Message = new byte[Data.Length + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, Message, 0, 4);
            Buffer.BlockCopy(Data, 0, Message, 4, Data.Length);

            foreach (var Client in ConnectedClients)
                Client.SendAsync(Message, SocketFlags.None);
        }
        #endregion

        #region Send
        /// <summary>
        /// Encrypt data(short) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, short Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(int) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, int Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(long) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, long Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(double) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, double Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(float) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, float Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(bool) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, bool Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(char) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, char Data)
            => SendEncrypted(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Encrypt data(string) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, string Data)
            => SendEncrypted(Client, _Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send it to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void SendEncrypted(Socket Client, byte[] Data)
            => Send(Client, (Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, short Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(int) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, int Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(long) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, long Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(double) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, double Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(float) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, float Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(bool) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, bool Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(char) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, char Data)
            => Send(Client, BitConverter.GetBytes(Data));
        /// <summary>
        /// Send data(string) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, string Data)
            => Send(Client, _Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to 1 client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send to client</param>
        public void Send(Socket Client, byte[] Data)
        {
            if (Data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (!IsRunning) throw new Exception("Could not send data: Server is not running.");

            byte[] Message = new byte[Data.Length + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, Message, 0, 4);
            Buffer.BlockCopy(Data, 0, Message, 4, Data.Length);

            Client.SendAsync(Message, SocketFlags.None);
        }
        #endregion

        #region SendAndGetReply
        /// <summary>
        /// Encrpt data(short) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, short Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(int) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, int Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(long) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, long Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(double) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, double Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(float) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, float Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(bool) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, bool Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(char) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, char Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Encrpt data(string) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, string Data, TimeSpan Timeout)
            => SendAndGetReplyEncrypted(Client, _Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);
        /// <summary>
        /// Encrpt data(byte[]) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket Client, byte[] Data, TimeSpan Timeout)
            => SendAndGetReply(Client, (Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);

        /// <summary>
        /// Send data(short) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, short Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(int) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, int Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(long) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, long Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(double) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, double Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(float) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, float Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(bool) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, bool Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(char) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, char Data, TimeSpan Timeout)
            => SendAndGetReply(Client, BitConverter.GetBytes(Data), Timeout);
        /// <summary>
        /// Send data(string) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, string Data, TimeSpan Timeout)
            => SendAndGetReply(Client, _Encoding.GetBytes(Data ?? throw new ArgumentNullException("Could not send data: Data is null.")), Timeout);
        /// <summary>
        /// Send data(byte[]) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="Client">Client to send data to</param>
        /// <param name="Data">Data to send</param>
        /// <param name="Timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket Client, byte[] Data, TimeSpan Timeout)
        {
            if (Timeout.Ticks.Equals(0)) throw new ArgumentException("Invalid Timeout.");

            Message Reply = null;
            ManualResetEventSlim Signal = new ManualResetEventSlim();

            void Event(object sender, Message e) { if (e.Socket.Equals(Client)) { Reply = e; DataReceived -= Event; Signal.Set(); } };

            DataReceived += Event;
            Send(Client, Data);

            Signal.Wait(Timeout);
            return Reply;
        }
        #endregion

        /*This functions are used by the ServerListener class*/
        internal void NotifyClientConnected(Socket Client) => ClientConnected?.Invoke(this, Client);
        internal void NotifyClientDisconnected(Socket Client) => ClientDisconnected?.Invoke(this, Client);
        internal void NotifyDataReceived(byte[] Data, Socket Client) => DataReceived?.Invoke(this, new Message(Data, Client, Encryption, _Encoding));
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
        internal void NotifyClientRefused(RefusedClient BannedClient) => ClientRefused?.Invoke(this, BannedClient);
    }
}

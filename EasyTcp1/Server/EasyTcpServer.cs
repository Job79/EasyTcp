/* EasyTcp1
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
        private ServerListener serverListener;

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
        /// BannedIPs, refuse all ip's in this list.
        /// </summary>
        public HashSet<string> BannedIPs = new HashSet<string>();

        /// <summary>
        /// Convert string to IPAddress.
        /// Used by the Start overloads.
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
        /// Start the server and overide encryption.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="port">Port as ushort(0-65,535)</param>
        /// <param name="maxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="encryption">Encryption class <see cref="EasyTcp.Encryption"/></param>
        /// <param name="dualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="maxDataSize">Max size of a message the server can receive</param>
        public void Start(string IP, ushort port, int maxConnections, Encryption encryption, bool dualMode = false, ushort maxDataSize = 1024)
        {
            Encryption = encryption;
            Start(GetIP(IP), port, maxConnections, dualMode, maxDataSize);
        }
        /// <summary>
        /// Start the server and overide encryption.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="port">Port as ushort(0-65,535)</param>
        /// <param name="maxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="encryption">Encryption class <see cref="EasyTcp.Encryption"/></param>
        /// <param name="dualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="maxDataSize">Max size of a message the server can receive</param>
        public void Start(IPAddress IP, ushort port, int maxConnections, Encryption encryption, bool dualMode = false, ushort maxDataSize = 1024)
        {
            Encryption = encryption;
            Start(IP, port, maxConnections, dualMode, maxDataSize);
        }
        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="IP">IP address as string</param>
        /// <param name="port">Port as ushort(0-65,535)</param>
        /// <param name="maxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="dualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="maxDataSize">Max size of a message the server can receive</param>
        public void Start(string IP, ushort port, int maxConnections, bool dualMode = false, ushort maxDataSize = 1024)
            => Start(GetIP(IP), port, maxConnections, dualMode, maxDataSize);
        /// <summary>
        /// Start the server.
        /// </summary>
        /// <param name="IP">IP address as IPAddress</param>
        /// <param name="port">Port as ushort(0-65,535)</param>
        /// <param name="maxConnections">MaxConnectedCount, client will be refused if the maximum is reached</param>
        /// <param name="dualMode">DualMode will specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6. DualMode sockets need to be started with an IPv6 address</param>
        /// <param name="maxDataSize">Max size of a message the server can receive</param>
        public void Start(IPAddress IP, ushort port, int maxConnections, bool dualMode = false, ushort maxDataSize = 1024)
        {
            if (IsRunning) throw new Exception("Could not start server: Server is already running.");
            else if (IP == null) throw new ArgumentNullException("Could not start server: Ip is null");
            else if (port == 0) throw new ArgumentException("Could not start server: Invalid Port.");
            else if (maxConnections <= 0) throw new ArgumentException("Could not start server: Invalid MaxConnections count.");
            else if (maxDataSize == 0) throw new ArgumentException("Could not start server: Invalid MaxDataSize.");

            Socket listener = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (IP.AddressFamily == AddressFamily.InterNetworkV6) listener.DualMode = dualMode;
            listener.Bind(new IPEndPoint(IP, port));

            //Create class ServerListener, this will start the passed socket and handle the events.
            serverListener = new ServerListener(listener, this, maxConnections, maxDataSize);
        }

        /// <summary>
        /// Return the ConnectedClients.
        /// </summary>
        public IEnumerable<Socket> ConnectedClients
        {
            get
            {
                if (!IsRunning) return null;
                else return serverListener.ConnectedClients.ToList();//.ToList because ConnectedClients is used by async funtions.
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
                else return serverListener.ConnectedClients.Count;
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
                else return serverListener.Listener;
            }
        }

        /// <summary>
        /// Return the state of the server.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return serverListener != null;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                serverListener.IsListerning = false;
                serverListener.Listener.Close();
                serverListener = null;
            }
        }

        /// <summary>
        /// Kick a Socket.
        /// </summary>
        public void Kick(Socket client)
            => (client ?? throw new ArgumentNullException("Could not kick client: Socket is null")).Shutdown(SocketShutdown.Both);

        /// <summary>
        /// Add the IP of the socket to BannedIPs and kick the client.
        /// </summary>
        public void Ban(Socket client)
        {
            BannedIPs.Add(((IPEndPoint)(client ?? throw new ArgumentNullException("Could not ban client: Socket is null")).RemoteEndPoint).Address.ToString());//Add client IP to banned IPs
            Kick(client);//Kick client
        }

        #region Broadcast
        /// <summary>
        /// Encrypt data(short) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(short data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(int) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(int data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(long) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(long data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(double) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(double data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(float) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(float data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(bool) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(bool data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(char) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(char data)
            => BroadcastEncrypted(BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(string) and send it to all clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void BroadcastEncrypted(string data)
            => BroadcastEncrypted(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send it to all clients.
        /// </summary>
        /// <param name="Data">Data to send to clients</param>
        public void BroadcastEncrypted(byte[] data)
            => Broadcast((Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(short data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(int) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(int data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(long) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(long data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(double) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(double data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(float) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(float data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(bool) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(bool data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(char) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(char data)
            => Broadcast(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(string) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(string data)
            => Broadcast(encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to all connected clients.
        /// </summary>
        /// <param name="data">Data to send to clients</param>
        public void Broadcast(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (!IsRunning)
            { NotifyOnError(new Exception("Could not send data: Server is not running.")); return; }

            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);

            using (SocketAsyncEventArgs e = new SocketAsyncEventArgs())
            {
                e.SetBuffer(message, 0, message.Length);

                foreach (var client in ConnectedClients)
                    client.SendAsync(e);
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Encrypt data(short) and send it to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, short data)
            => SendEncrypted(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(int) and send it to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, int data)
            => SendEncrypted(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(long) and send it to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, long data)
            => SendEncrypted(client, BitConverter.GetBytes(data));
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
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, float data)
            => SendEncrypted(client, BitConverter.GetBytes(data));
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
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, char data)
            => SendEncrypted(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Encrypt data(string) and send it to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, string data)
            => SendEncrypted(client, encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Encrypt data(byte[]) and send it to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void SendEncrypted(Socket client, byte[] data)
            => Send(client, (Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));

        /// <summary>
        /// Send data(short) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, short data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(int) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, int data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(long) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, long data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(double) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, double data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(float) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, float data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(bool) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, bool data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(char) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, char data)
            => Send(client, BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(string) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, string data)
            => Send(client, encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")));
        /// <summary>
        /// Send data(byte[]) to 1 client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send to client</param>
        public void Send(Socket client, byte[] data)
        {
            if (data == null) throw new ArgumentNullException("Could not send data: Data is null.");
            else if (!IsRunning) throw new Exception("Could not send data: Server is not running.");

            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);

            using (SocketAsyncEventArgs e = new SocketAsyncEventArgs())
            {
                e.SetBuffer(message, 0, message.Length);
                client.SendAsync(e);//Write async so it won't block UI applications.
            }
        }
        #endregion

        #region SendAndGetReply
        /// <summary>
        /// Encrpt data(short) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, short data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(int) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, int data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(long) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, long data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(double) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, double data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(float) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, float data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(bool) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, bool data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(char) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, char data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Encrpt data(string) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, string data, TimeSpan timeout)
            => SendAndGetReplyEncrypted(client, encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);
        /// <summary>
        /// Encrpt data(byte[]) and send data to 1 client, then wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReplyEncrypted(Socket client, byte[] data, TimeSpan timeout)
            => SendAndGetReply(client, (Encryption ?? throw new NullReferenceException("Could not encrypt data: Encryption class is null.")).Encrypt(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);

        /// <summary>
        /// Send data(short) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, short data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(int) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, int data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(long) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, long data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(double) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, double data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
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
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, bool data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(char) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, char data, TimeSpan timeout)
            => SendAndGetReply(client, BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(string) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, string data, TimeSpan timeout)
            => SendAndGetReply(client, encoding.GetBytes(data ?? throw new ArgumentNullException("Could not send data: Data is null.")), timeout);
        /// <summary>
        /// Send data(byte[]) to 1 client and wait for a reply from the client.
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="data">Data to send</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>The reply of the client</returns>
        public Message SendAndGetReply(Socket client, byte[] data, TimeSpan timeout)
        {
            if (timeout.Ticks.Equals(0)) throw new ArgumentException("Invalid Timeout.");

            Message reply = null;
            using (ManualResetEventSlim signal = new ManualResetEventSlim())
            {
                void Event(object sender, Message e) { if (e.Socket.Equals(client)) { reply = e; DataReceived -= Event; signal.Set(); } };

                DataReceived += Event;
                Send(client, data);

                signal.Wait(timeout);
                return reply;
            }
        }
        #endregion

        /*This functions are used by the ServerListener class*/
        internal void NotifyClientConnected(Socket client) => ClientConnected?.Invoke(this, client);
        internal void NotifyClientDisconnected(Socket client) => ClientDisconnected?.Invoke(this, client);
        internal void NotifyDataReceived(byte[] data, Socket client) => DataReceived?.Invoke(this, new Message(data, client, Encryption, encoding));
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
        internal void NotifyClientRefused(RefusedClient bannedClient) => ClientRefused?.Invoke(this, bannedClient);
    }
}

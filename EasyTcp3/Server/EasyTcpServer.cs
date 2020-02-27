using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EasyTcp3.Client;

namespace EasyTcp3.Server
{
    public class EasyTcpServer : IDisposable
    {
        /// <summary>
        /// Server socket
        /// </summary>
        public Socket BaseSocket { get; protected set; }
        
        /// <summary>
        /// Determines if the server is currently running
        /// </summary>
        private bool _isRunning;
        public bool IsRunning => _isRunning;


        /// <summary>
        /// List of all connected clients
        /// </summary>
        private readonly HashSet<Socket> _connectedClients = new HashSet<Socket>();
        public IEnumerable<Socket> ConnectedClients => _connectedClients.ToList(); //ToList to create deep copy

        public event EventHandler<EasyTcpClient> OnClientConnect;
        public event EventHandler<EasyTcpClient> OnClientDisconnect;
        public event EventHandler<Message> OnDataReceive;
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Encoding used for strings
        /// </summary>
        private Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentException("Encoding can't be set to null");
        }

        /// <summary>
        /// Start the server
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="dualMode"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Start(IPAddress address, ushort port, bool dualMode = false)
        {
            if (IsRunning) throw new Exception("Could not start server: Server is already running");
            else if (address == null) throw new ArgumentException("Could not start server: Address is null");
            else if (port == 0) throw new ArgumentException("Could not start server: Invalid Port");

            BaseSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (address.AddressFamily == AddressFamily.InterNetworkV6) BaseSocket.DualMode = dualMode;
            BaseSocket.Bind(new IPEndPoint(address, port));
            BaseSocket.Listen(100); //100 = maximum pending connections
            BaseSocket.BeginAccept(_OnClientConnect, null); //Start accepting new connections
            _isRunning = true;
        }

        /// <summary>
        /// Dispose/stop the server
        /// </summary>
        public void Dispose()
        {
            if(BaseSocket == null) return;
            _isRunning = false;
            _connectedClients.Clear();
            BaseSocket.Dispose();
            BaseSocket = null;
        }

        /// <summary>
        /// Async function, triggered when a client connects
        /// </summary>
        /// <param name="ar"></param>
        private void _OnClientConnect(IAsyncResult ar)
        {
            if (!IsRunning) return;

            try
            {
                var client = BaseSocket.EndAccept(ar); //Accept socket.
                var clientObject = new ClientObject {Socket = client, Buffer = new byte[2]};

                _connectedClients.Add(client);
                OnClientConnect?.Invoke(this,new EasyTcpClient(client));
                client.BeginReceive(clientObject.Buffer, 0, clientObject.Buffer.Length, SocketFlags.None,
                    OnReceive, clientObject);
            }
            catch (Exception ex) { if (IsRunning) NotifyOnError(ex); }

            BaseSocket.BeginAccept(_OnClientConnect, null); //Accept next client
        }

        /// <summary>
        /// Async function, triggered when receiving data or client disconnects
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            if (!IsRunning) return;
            var client = ar.AsyncState as ClientObject;

            try
            {
                ushort dataLength = 2;
                if (client.ReceiveData = !client.ReceiveData) //If receiving length
                {
                    if ((dataLength = BitConverter.ToUInt16(client.Buffer, 0)) == 0)
                    {
                        HandleDisconnect(client);
                        return;
                    }
                }
                else OnDataReceive?.Invoke(this, new Message(client.Buffer, new EasyTcpClient(client.Socket))); //Trigger event

                client.Socket.BeginReceive(client.Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
                    OnReceive, client); //Start accepting the data.
            }
            catch (SocketException) { HandleDisconnect(client); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        private void HandleDisconnect(ClientObject client)
        {
            lock (ConnectedClients) _connectedClients.Remove(client.Socket);
            OnClientDisconnect?.Invoke(this, new EasyTcpClient(client.Socket));
            client.Socket.Close();
        }
        
        /*This function is used to handle errors*/
        private void NotifyOnError(Exception ex)
        {
            if (OnError != null) OnError(this, ex);
            else throw ex;
        }
    }
}
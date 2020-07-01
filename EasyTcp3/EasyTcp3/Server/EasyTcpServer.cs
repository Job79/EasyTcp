using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp3.Protocols;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3.Server
{
    /// <summary>
    /// EasyTcp server,
    /// Provides a simple high performance tcp server
    /// </summary>
    public class EasyTcpServer : IDisposable
    {
        /// <summary>
        /// BaseSocket of server
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }

        /// <summary>
        /// Protocol for server, protocol determines all behavior of this server
        /// </summary>
        public IEasyTcpProtocol Protocol
        {
            get => _protocol;
            set
            {
                if (IsRunning || BaseSocket != null)
                    throw new Exception("Can not change protocol when server is running.");
                _protocol = value;
            }
        }

        private IEasyTcpProtocol _protocol;

        /// <summary>
        /// Function used by send functions to Serialize objects
        /// </summary>
        public Func<object, byte[]> Serialize = o =>
            throw new Exception("Assign a function to serialize first before using serialisation");

        /// <summary>
        /// Function used by receive to Deserialize byte[] to object 
        /// </summary>
        public Func<byte[], Type, object> Deserialize = (b, t) =>
            throw new Exception("Assign a function to deserialize first before using serialisation");

        /// <summary>
        /// Determines whether the server is running,
        /// set to true when server is started, set to false before server is disposed
        /// </summary>
        public bool IsRunning { get; protected internal set; }

        /// <summary>
        /// Unsafe list with connected clients
        /// </summary>
        public List<EasyTcpClient> UnsafeConnectedClients = new List<EasyTcpClient>();

        /// <summary>
        /// Number of connected clients
        /// </summary>
        public int ConnectedClientsCount => UnsafeConnectedClients.Count;

        /// <summary>
        /// List with connected clients
        /// </summary>
        /// <returns>copy of UnsafeConnectedClients</returns>
        public List<EasyTcpClient> GetConnectedClients() => UnsafeConnectedClients.ToList();

        /// <summary>
        /// List with connected sockets 
        /// </summary>
        /// <returns>Copy of UnsafeConnectedClients</returns>
        public List<Socket> GetConnectedSockets() => GetConnectedClients().Select(c => c.BaseSocket).ToList();

        /// <summary>
        /// Event that is fired when a new client connects to the server
        /// Dispose client to dismiss connection
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Event that is fired when a client disconnects from server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Event that is fired when server receives data
        /// </summary>
        public event EventHandler<Message> OnDataReceive;
        
        /// <summary>
        /// Event that is fired when client receives data from remote host
        /// </summary>
        public event OnDataReceiveAsyncDelegate OnDataReceiveAsync;
        
        /// <summary>
        /// Delegate type for OnDataReceiveAsync
        /// </summary>
        public delegate Task OnDataReceiveAsyncDelegate(object sender, Message message);

        /// <summary>
        /// Event that is fired when server sends data to a client
        /// </summary>
        public event EventHandler<Message> OnDataSend;
        
        /// <summary>
        /// Event that is fired when error occurs
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Fire the OnConnect event 
        /// </summary>
        /// <param name="client"></param>
        public void FireOnConnect(EasyTcpClient client) => OnConnect?.Invoke(this, client);

        /// <summary>
        /// Fire the OnDisconnect event
        /// </summary>
        /// <param name="client"></param>
        public void FireOnDisconnect(EasyTcpClient client)
        {
            lock (UnsafeConnectedClients) UnsafeConnectedClients.Remove(client);
            OnDisconnect?.Invoke(this, client);
        }

        /// <summary>
        /// Fire the OnDataReceive event
        /// </summary>
        /// <param name="message"></param>
        public async Task FireOnDataReceive(Message message)
        {
            if (OnDataReceiveAsync != null) await OnDataReceiveAsync.Invoke(this, message);
            OnDataReceive?.Invoke(this, message);
        }

        /// <summary>
        /// Fire the OnDataSend event
        /// </summary>
        /// <param name="message"></param>
        public void FireOnDataSend(Message message) => OnDataSend?.Invoke(this, message);

        /// <summary>
        /// Fire the OnError event,
        /// throw error if event is not used and library is compiled with debug mode
        /// </summary>
        /// <param name="exception"></param>
        public void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        /// <summary></summary>
        /// <param name="protocol"></param>
        public EasyTcpServer(IEasyTcpProtocol protocol = null)
            => this.Protocol = protocol ?? new PrefixLengthProtocol();

        /// <summary>
        /// Dispose current instance of baseSocket if not null
        /// </summary>
        public void Dispose()
        {
            if (BaseSocket == null) return;
            IsRunning = false;
            lock (UnsafeConnectedClients)
            {
                foreach (var client in UnsafeConnectedClients) client.Dispose();
            }

            UnsafeConnectedClients.Clear();
            Protocol?.Dispose();
            BaseSocket.Dispose();
            BaseSocket = null;
        }
    }
}
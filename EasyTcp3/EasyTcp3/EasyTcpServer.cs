using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp3.Protocols;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3
{
    /// <summary>
    /// EasyTcp server
    /// Class with all EasyTcpServer properties and basic functions.
    /// See the "ServerUtils" folder for all the other functions.
    /// </summary>
    public class EasyTcpServer : IDisposable
    {
        /// <summary>
        /// BaseSocket of server 
        /// null if server isn't running.
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }
        
        /// <summary>
        /// Protocol used for this server and all connected clients 
        /// The used protocol determines the internal behavior of the server. 
        /// The protocol can't be changed when the server is running. 
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
        /// Function used by send functions to Serialize custom objects
        /// </summary>
        public Func<object, byte[]> Serialize = o =>
            throw new Exception("Assign a function to serialize first before using serialisation");

        /// <summary>
        /// Function used by receive to Deserialize byte[] to custom object 
        /// </summary>
        public Func<byte[], Type, object> Deserialize = (b, t) =>
            throw new Exception("Assign a function to deserialize first before using serialisation");
        
        /// <summary>
        /// Determines whether the server is running
        /// set to true when server is started, set to false before server is disposed
        /// </summary>
        public bool IsRunning { get; protected internal set; }

        /// <summary>
        /// Non-thread safe list with connected clients
        /// Use with caution!
        /// </summary>
        public List<EasyTcpClient> UnsafeConnectedClients = new List<EasyTcpClient>();

        /// <summary>
        /// Number of connected clients
        /// </summary>
        public int ConnectedClientsCount 
        {
            get
            {
                lock (UnsafeConnectedClients) return UnsafeConnectedClients.Count;
            }
        }

        /// <summary>
        /// List with connected clients
        /// </summary>
        /// <returns>copy of UnsafeConnectedClients</returns>
        public List<EasyTcpClient> GetConnectedClients()
        {
            lock(UnsafeConnectedClients) return UnsafeConnectedClients.ToList();
        }

        /// <summary>
        /// List with connected sockets 
        /// </summary>
        /// <returns>Copy of UnsafeConnectedClients</returns>
        public List<Socket> GetConnectedSockets() => GetConnectedClients().Select(c => c.BaseSocket).ToList();

        /// <summary>
        /// Event that is fired when a client connects to the server
        /// Dispose client to dismiss connection attempt.
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Event that is fired when a client disconnects from server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Event that is fired when server receives data from a client
        /// </summary>
        public event EventHandler<Message> OnDataReceive;
        
        /// <summary>
        /// Async event that is fired when server receives data from a client 
        /// </summary>
        public event EasyTcpClient.OnDataReceiveAsyncDelegate OnDataReceiveAsync;
        
        /// <summary>
        /// Event that is fired when server sends data to a client
        /// </summary>
        public event EventHandler<Message> OnDataSend;
        
        /// <summary>
        /// Event that is fired when an (internal) error occurs within the server or a connected client
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Fire the OnConnect event and add client to UnsafeConnectedClients
        /// </summary>
        /// <param name="client"></param>
        public void FireOnConnect(EasyTcpClient client)
        {
            OnConnect?.Invoke(this, client);
            if (client.BaseSocket != null) // Check if user aborted OnConnect with Client.Dispose()
                lock (UnsafeConnectedClients)
                    UnsafeConnectedClients.Add(client);
        }

        /// <summary>
        /// Fire the OnDisconnect event and remove client from UnsafeConnectedClients
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
        /// throw error if event handler isn't used
        /// </summary>
        /// <param name="exception"></param>
        public void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        /// <summary>
        /// Construct new EasyTcpServer
        /// </summary>
        /// <param name="protocol"></param>
        public EasyTcpServer(IEasyTcpProtocol protocol = null)
        {
            Protocol = protocol ?? new PrefixLengthProtocol();
#if (NETCOREAPP3_1 || NET5_0)
            Serialize = o => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(o);
            Deserialize = (b, t) => System.Text.Json.JsonSerializer.Deserialize(b, t);
#endif
        }

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
            Protocol = null;
        }
    }
}

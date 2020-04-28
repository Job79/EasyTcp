using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace EasyTcp3.Server
{
    public class EasyTcpServer : IDisposable
    {
        
        public Socket BaseSocket { get; protected internal set; }
        /// <summary>
        /// Gets a value that determines if this client is connected to a host
        /// </summary>
        public bool IsRunning { get; protected internal set; }

        /// <summary>
        /// List with all the connected clients
        /// </summary>
        protected internal HashSet<EasyTcpClient> ConnectedClients = new HashSet<EasyTcpClient>();
        /// <summary>
        /// Get the number of connected clients
        /// </summary>
        public int ConnectedClientsCount => ConnectedClients.Count;
        /// <summary>
        /// List of all connected clients
        /// Creates copy of ConnectedClients because this variable is used by async functions
        /// </summary>
        /// <returns>Copy of ConnectedClients</returns>
        public IEnumerable<EasyTcpClient> GetConnectedClients() => ConnectedClients.ToList();
        /// <summary>
        /// List of all connected sockets
        /// Creates copy of ConnectedClients because this variable is used by async functions
        /// </summary>
        /// <returns>Copy of the sockets in ConnectedClients</returns>
        public IEnumerable<Socket> GetConnectedSockets() => GetConnectedClients().Select(c=>c.BaseSocket);

        /// <summary>
        /// Fired when the client connects to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;
        
        protected internal void FireOnConnect(EasyTcpClient client) => OnConnect?.Invoke(this, client);
        protected internal void FireOnError(Exception e)
        {
            if (OnError != null) OnError.Invoke(this, e);
            else throw e;
        }
        
        /// <summary>
        /// Dispose current instance of the baseSocket
        /// </summary>
        public void Dispose()
        {
            if(BaseSocket == null) return;
            IsRunning = false;
            foreach (var client in ConnectedClients) client.Dispose();
            ConnectedClients.Clear();
            
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}
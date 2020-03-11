using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using EasyTcp3.Server;

namespace EasyTcp3
{
    public class EasyTcpServer : IDisposable
    {
        public Socket BaseSocket { get; protected internal set; }
        /// <summary>
        /// Gets a value that determines if this client is connected to a host
        /// </summary>
        public bool IsRunning { get; protected internal set; }

        /// <summary>
        /// Fired when the client connects to the server
        /// </summary>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///
        /// int connectCount = 0;
        /// server.OnConnect += (sender, client) =>
        /// {
        ///    Interlocked.Increment(ref connectCount);//Async lambda, thread safe increase integer
        ///    Console.WriteLine($"Client {connectCount} connected");
        /// };
        ///
        /// using var client = new EasyTcpClient();
        /// Assert.IsTrue(client.Connect(IPAddress.Any, port));
        /// 
        /// TestHelper.WaitWhileTrue(()=>connectCount == 0);
        /// Assert.AreEqual(1,connectCount);
        /// </example>
        public event EventHandler<EasyTcpClient> OnConnect;
        protected internal void FireOnConnect(EasyTcpClient client) => OnConnect?.Invoke(this, client);

        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;
        protected internal void FireOnError(Exception e)
        {
            if (OnError != null) OnError.Invoke(this, e);
            else throw e;
        }

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
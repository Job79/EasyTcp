using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp3.Server;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Implementation of tcp protocol
    /// </summary>
    public abstract class DefaultTcpProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// AsyncEventArgs with received data 
        /// </summary>
        public SocketAsyncEventArgs ReceiveBuffer;
        
        /// <summary>
        /// Default socket for protocol
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of socket compatible with protocol</returns>
        public virtual Socket GetSocket(AddressFamily addressFamily) =>
            new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        /// <summary>
        /// Start accepting new clients
        /// </summary>
        /// <param name="server"></param>
        public virtual void StartAcceptingClients(EasyTcpServer server)
        {
            if (server.AcceptArgs == null)
            {
                server.BaseSocket.Listen(50000);
                server.AcceptArgs = new SocketAsyncEventArgs {UserToken = server};
                server.AcceptArgs.Completed += (_, ar) => OnConnectCallback(ar);
            }

            server.AcceptArgs.AcceptSocket = null;
            if (!server.BaseSocket.AcceptAsync(server.AcceptArgs)) OnConnectCallback(server.AcceptArgs);
        }

        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public virtual void EnsureDataReceiverIsRunning(EasyTcpClient client)
        {
            if (IsListening) return;
            IsListening = true;
                                                                
            var protocol = (DefaultTcpProtocol) client.Protocol;
                                               
            if (protocol.ReceiveBuffer == null)
            {                                                                          
                protocol.ReceiveBuffer = new SocketAsyncEventArgs {UserToken = client};
                protocol.ReceiveBuffer.Completed += (_, ar) => OnReceiveCallback(ar);
            }
                                        
            var bufferSize = BufferSize;                                          
            protocol.ReceiveBuffer.SetBuffer(new byte[bufferSize], 0, bufferSize);                                 
            if (!client.BaseSocket.ReceiveAsync(protocol.ReceiveBuffer)) OnReceiveCallback(protocol.ReceiveBuffer);

        }

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// returned data will be send to remote host
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public abstract byte[] CreateMessage(params byte[][] data);
        
        /// <summary>
        /// Send message to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public virtual void SendMessage(EasyTcpClient client, byte[] message)
        {
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            client.FireOnDataSend(message, client);
            client.BaseSocket.Send(message, SocketFlags.None);
        }
        
        /// <summary>
        /// Get receiving/sending stream
        /// </summary>
        /// <returns></returns>
        public Stream GetStream(EasyTcpClient client) => new NetworkStream(client.BaseSocket);
        
        /// <summary>
        /// Method that is triggered when client connects
        /// Default behavior is starting listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public bool OnConnect(EasyTcpClient client)
        {
            EnsureDataReceiverIsRunning(client);
            return true;
        }

        /// <summary>
        /// Method that is triggered when client connects to server
        /// Default behavior is starting listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public bool OnConnectServer(EasyTcpClient client)
        {
            EnsureDataReceiverIsRunning(client);
            return true;
        }
        
        /// <summary>
        /// Create new instance of current protocol,
        /// used by server when accepting a new client
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
        
        /// <summary>
        /// Dispose current instance, ignored by DefaultTcpProtocol 
        /// </summary>
        public virtual void Dispose()
        {
            ReceiveBuffer?.Dispose();
        }
        
        /*
         * Methods used by internal receivers that need to be implemented when using this class 
         */
        
        /// <summary>
        /// Size of (next) buffer used by receive event 
        /// </summary>
        public abstract int BufferSize { get; protected set; }

        /// <summary>
        /// Handle received data, function should call <code>EasyTcpClient.DataReceiveHandler({Received message});</code> 
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public abstract Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client);

        /*
         * Internal methods
         */

        /// <summary>
        /// Determines whether the DataReceiver is started
        /// </summary>
        protected bool IsListening;

        /// <summary>
        /// Fire OnDisconnectEvent and dispose client 
        /// </summary>
        /// <param name="client"></param>
        protected virtual void HandleDisconnect(EasyTcpClient client)
        {
            client.FireOnDisconnect();
            client.Dispose();
        }

        /// <summary>
        /// Callback method that accepts new tcp connections
        /// Fired when new client connects
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void OnConnectCallback(SocketAsyncEventArgs ar)
        {
            var server = ar.UserToken as EasyTcpServer;
            if (server?.BaseSocket == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(ar.AcceptSocket,
                    (IEasyTcpProtocol) server.Protocol.Clone())
                {
                    Serialize = server.Serialize,
                    Deserialize = server.Deserialize
                };
                client.OnDataReceiveAsync += async (_, message) => await server.FireOnDataReceive(message);
                client.OnDataSend += (_, message) => server.FireOnDataSend(message);
                client.OnDisconnect += (_, c) => server.FireOnDisconnect(c);
                client.OnError += (_, exception) => server.FireOnError(exception);

                StartAcceptingClients(server);

                if (!client.Protocol.OnConnectServer(client)) return;
                server.FireOnConnect(client);
                if (client.BaseSocket != null) // Check if user aborted OnConnect with Client.Dispose()
                    lock (server.UnsafeConnectedClients)
                        server.UnsafeConnectedClients.Add(client);
            }
            catch (Exception ex)
            {
                server.FireOnError(ex);
            }
        }

        /// <summary>
        /// Callback method that handles receiving data
        /// Fired when new data is received
        /// </summary>
        /// <param name="ar"></param>
        protected virtual async void OnReceiveCallback(SocketAsyncEventArgs ar)
        {
            var client = ar.UserToken as EasyTcpClient;
            if (client == null) return;
            IsListening = false;

            try
            {
                if (ar.BytesTransferred != 0)
                {
                    await DataReceive(ar.Buffer, ar.BytesTransferred, client);
                    if (client.BaseSocket == null) HandleDisconnect(client); // Check if client is disposed by DataReceive
                    else EnsureDataReceiverIsRunning(client);
                }
                else HandleDisconnect(client);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                    HandleDisconnect(client);
                else if (client?.BaseSocket != null) client.FireOnError(ex);
            }
        }
    }
}
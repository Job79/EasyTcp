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
            server.BaseSocket.Listen(50000);
            server.BaseSocket.BeginAccept(OnConnectCallback, server);
        }

        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public virtual void EnsureDataReceiverIsRunning(EasyTcpClient client)
        {
            if(IsListening) return;
            IsListening = true;
            client.BaseSocket.BeginReceive(client.Buffer = new byte[BufferSize], 0,
                client.Buffer.Length, SocketFlags.None, OnReceiveCallback, client);
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

            client.FireOnDataSend(new Message(message, client));
            client.BaseSocket.BeginSend(message, 0, message.Length, SocketFlags.None, ar =>
            {
                var socket = ar.AsyncState as Socket;
                socket?.EndSend(ar);
            }, client.BaseSocket);
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
        protected virtual void OnConnectCallback(IAsyncResult ar)
        {
            var server = ar.AsyncState as EasyTcpServer;
            if (server?.BaseSocket == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(server.BaseSocket.EndAccept(ar),
                    (IEasyTcpProtocol) server.Protocol.Clone())
                {
                    Serialize = server.Serialize,
                    Deserialize = server.Deserialize
                };
                client.OnDataReceiveAsync += async (_, message) => await server.FireOnDataReceive(message);
                client.OnDataSend += (_, message) => server.FireOnDataSend(message);
                client.OnDisconnect += (_, c) => server.FireOnDisconnect(c);
                client.OnError += (_, exception) => server.FireOnError(exception);
                server.BaseSocket.BeginAccept(OnConnectCallback, server);

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
        protected virtual async void OnReceiveCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client == null) return;
            IsListening = false;

            try
            {
                int receivedBytes = client.BaseSocket.EndReceive(ar, out SocketError socketErr);
                if (receivedBytes != 0 || socketErr != SocketError.Success)
                {
                    await DataReceive(client.Buffer, receivedBytes, client);
                    if (client.BaseSocket == null)
                        HandleDisconnect(client); // Check if client is disposed by DataReceive
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
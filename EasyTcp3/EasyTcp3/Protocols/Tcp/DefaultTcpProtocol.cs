using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EasyTcp3.Protocols.Tcp
{
    /// <summary>
    /// Abstract implementation of the tcp protocol
    /// Extend DefaultTcpProtocol when using tcp, see PrefixLengthProtocol, DelimiterProtocol or NoneProtocol for examples
    /// </summary>
    public abstract class DefaultTcpProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// AsyncEventArgs used to receive new data 
        /// </summary>
        protected SocketAsyncEventArgs ReceiveBuffer;

        /// <summary>
        /// AsyncEventArgs used to accept new connections (null for clients)
        /// </summary>
        protected SocketAsyncEventArgs AcceptArgs;

        /// <summary>
        /// Determines whether the DataReceiver is started
        /// </summary>
        protected bool IsListening;

        /// <summary>
        /// NetworkStream used by getStream()
        /// Null when getStream() isn't used
        /// </summary>
        protected NetworkStream NetworkStream;

        /// <summary>
        /// Get new instance of a tcp socket 
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of socket compatible with the tcp protocol</returns>
        public virtual Socket GetSocket(AddressFamily addressFamily) =>
            new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Get receiving/sending stream
        /// </summary>
        /// <returns></returns>
        public virtual Stream GetStream(EasyTcpClient client) => NetworkStream ??= new NetworkStream(client.BaseSocket);

        /// <summary>
        /// Start accepting new clients
        /// </summary>
        /// <param name="server"></param>
        public virtual void StartAcceptingClients(EasyTcpServer server)
        {
            if (AcceptArgs == null)
            {
                server.BaseSocket.Listen(50000);
                AcceptArgs = new SocketAsyncEventArgs {UserToken = server};
                AcceptArgs.Completed += (_, ar) => OnConnectCallback(ar);
            }

            AcceptArgs.AcceptSocket = null;
            if (!server.BaseSocket.AcceptAsync(AcceptArgs)) OnConnectCallback(AcceptArgs);
        }

        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public virtual void EnsureDataReceiverIsRunning(EasyTcpClient client)
        {
            if (IsListening) return;
            IsListening = true;

            // Get protocol from client because this function can be used like this:
            // server.EnsureDataReceiverIsRunning(client)
            var p = (DefaultTcpProtocol) client.Protocol;

            if (p.ReceiveBuffer == null)
            {
                p.ReceiveBuffer = new SocketAsyncEventArgs {UserToken = client};
                p.ReceiveBuffer.Completed += (_, ar) => p.OnReceiveCallback(ar);
            }

            p.ReceiveBuffer.SetBuffer(new byte[p.BufferSize], p.BufferOffset, p.BufferCount);
            if (!client.BaseSocket.ReceiveAsync(p.ReceiveBuffer)) OnReceiveCallback(p.ReceiveBuffer);
        }

        /// <summary>
        /// Send message to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public virtual void SendMessage(EasyTcpClient client, byte[] message)
        {
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            client.FireOnDataSend(message);
            client.BaseSocket.Send(message, SocketFlags.None);
        }

        /// <summary>
        /// Method that is triggered when client connects to remote endpoint 
        /// </summary>
        /// <param name="client"></param>
        public virtual bool OnConnect(EasyTcpClient client)
        {
            EnsureDataReceiverIsRunning(client);
            return true;
        }

        /// <summary>
        /// Method that is triggered when server accepted a new client
        /// </summary>
        /// <param name="client"></param>
        public virtual bool OnConnectServer(EasyTcpClient client)
        {
            EnsureDataReceiverIsRunning(client);
            return true;
        }

        /// <summary>
        /// Dispose protocol, automatically called by client.Dispose and server.Dispose 
        /// </summary>
        public virtual void Dispose()
        {
            ReceiveBuffer?.Dispose();
            NetworkStream?.Dispose();
            AcceptArgs?.Dispose();
        }

        /*
         * Methods used by internal receivers that need to be implemented when using this class 
         */

        /// <summary>
        /// The size of the (next) buffer, used by receive event 
        /// </summary>
        public abstract int BufferSize { get; protected set; }

        /// <summary>
        /// TODO
        /// </summary>
        public virtual int BufferOffset { get => 0; protected set {} }

        /// <summary>
        /// TODO
        /// </summary>
        public virtual int BufferCount { get => BufferSize; protected set {} }

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// Returned data will be send to remote host.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public abstract byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Handle received data, function should call <code>EasyTcpClient.DataReceiveHandler({Received message});</code> to trigger the OnDataReceive events 
        /// </summary>
        /// <param name="data">received data, has the size of the clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes, can be smaller then the buffer</param>
        /// <param name="client"></param>
        public abstract Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client);

        /// <summary>
        /// Return new instance of protocol
        /// Used by the server to create copies for all connected clients.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /*
         * Internal methods
         */

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
        /// Fired when new client connects.
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void OnConnectCallback(SocketAsyncEventArgs ar)
        {
            var server = ar.UserToken as EasyTcpServer;
            if (server?.BaseSocket == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient((IEasyTcpProtocol) server.Protocol.Clone())
                {
                    BaseSocket = ar.AcceptSocket,
                    Serialize = server.Serialize,
                    Deserialize = server.Deserialize
                };
                client.OnDataReceiveAsync += async (_, message) => await server.FireOnDataReceive(message);
                client.OnDataSend += (_, message) => server.FireOnDataSend(message);
                client.OnDisconnect += (_, c) => server.FireOnDisconnect(c);
                client.OnError += (_, exception) => server.FireOnError(exception);

                StartAcceptingClients(server);
                if (client.Protocol.OnConnectServer(client)) server.FireOnConnect(client);
            }
            catch (Exception ex)
            {
                server.FireOnError(ex);
            }
        }

        /// <summary>
        /// Callback method that handles receiving data
        /// Fired when new data is received.
        /// </summary>
        /// <param name="ar"></param>
        protected virtual async void OnReceiveCallback(SocketAsyncEventArgs ar)
        {
            receive_data:
            var client = ar.UserToken as EasyTcpClient;
            if (client == null) return;
            IsListening = false;

            try
            {
                if (ar.BytesTransferred != 0)
                {
                    await DataReceive(ar.Buffer, ar.BytesTransferred, client);

                    if (client.BaseSocket == null)
                        HandleDisconnect(client); // Check if client is disposed by DataReceive
                    else if(!IsListening) /* Continue listening for data  */
                    {
                        IsListening = true;
                        
                        if(BufferOffset == 0 || BufferSize != ar.Buffer.Length) ar.SetBuffer(new byte[BufferSize], BufferOffset, BufferCount);
                        else ar.SetBuffer(BufferOffset, BufferCount);

                        if (!client.BaseSocket.ReceiveAsync(ar)) goto receive_data;
                    }
                }
                else HandleDisconnect(client);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                    HandleDisconnect(client);
                else if (client.BaseSocket != null) client.FireOnError(ex);
            }
        }
    }
}

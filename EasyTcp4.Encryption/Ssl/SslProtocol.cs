using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EasyTcp4.Protocols;

namespace EasyTcp4.Encryption.Ssl
{
    /// <summary>
    /// Abstract implementation of the tcp/ssl protocol
    /// Base class of the other ssl protocols
    /// </summary>
    public abstract class SslProtocol : IEasyProtocol
    {
        /// <summary>
        /// Buffer used to receive new data 
        /// </summary>
        protected byte[] ReceiveBuffer;

        /// <summary>
        /// AsyncEventArgs used to accept new connections (null for clients)
        /// </summary>
        protected SocketAsyncEventArgs AcceptArgs;

        /// <summary>
        /// Determines whether client is listening for data
        /// </summary>
        protected bool IsListening;

        /// <summary>
        /// Encrypted networkstream used for receiving / sending data
        /// </summary>
        protected SslStream SslStream;

        /// <summary>
        /// Unencrypted networkstream
        /// </summary>
        protected NetworkStream NetworkStream;

        /// <summary>
        /// Certificate used by SslStream
        /// ignored if protocol is used by a client
        /// </summary>
        protected readonly X509Certificate Certificate;

        /// <summary>
        /// ServerName used by SslStream.AuthenticateAsClient, must be the same as the servername in the server certificate
        /// ignored if protocol is used by a server
        /// </summary>
        protected readonly string ServerName;

        /// <summary>
        /// Determines whether the client can connect to servers that use an invalid certificate 
        /// ignored if protocol is used by a server
        /// </summary>
        protected readonly bool AcceptInvalidCertificates;

        /// <summary></summary>
        /// <param name="certificate">server certificate</param>
        public SslProtocol(X509Certificate certificate) => Certificate = certificate;

        /// <summary></summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client can connect to servers that use an invalid certificate</param>
        public SslProtocol(string serverName, bool acceptInvalidCertificates = false)
        {
            ServerName = serverName;
            AcceptInvalidCertificates = acceptInvalidCertificates;
        }

        /// <summary>
        /// Get a new instance of a socket compatible with protocol 
        /// Used by the Connect and Start utils
        /// </summary>
        public virtual Socket GetSocket(AddressFamily addressFamily)
            => new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Get receiving/sending stream
        /// Stream will not be disposed after use, reuse same stream and dispose stream in .Dispose function of protocol
        /// </summary>
        public virtual Stream GetStream(EasyTcpClient client) => SslStream;

        /// <summary>
        /// Start accepting new connections 
        /// </summary>
        public virtual void StartAcceptingClients(EasyTcpServer server)
        {
            if (AcceptArgs == null)
            {
                server.BaseSocket.Listen(1000);
                AcceptArgs = new SocketAsyncEventArgs { UserToken = server };
                AcceptArgs.Completed += (_, ar) => OnConnectCallback(ar);
            }

            AcceptArgs.AcceptSocket = null;
            if (!server.BaseSocket.AcceptAsync(AcceptArgs)) OnConnectCallback(AcceptArgs);
        }

        /// <summary>
        /// Start or continue listerning for incoming data
        /// </summary>
        public virtual void EnsureDataReceiverIsRunning(EasyTcpClient client)
        {
            if (IsListening) return;
            IsListening = true;

            if (BufferSize != ReceiveBuffer?.Length) ReceiveBuffer = new byte[BufferSize];
            SslStream.BeginRead(ReceiveBuffer, BufferOffset, BufferCount, OnReceiveCallback, client);
        }

        /// <summary>
        /// Method that is triggered when client connected to remote endpoint 
        /// Authenticate as client and start accepting new data
        /// </summary>
        public virtual bool OnConnect(EasyTcpClient client)
        {
            try
            {
                NetworkStream = new NetworkStream(client.BaseSocket);
                SslStream = new SslStream(NetworkStream, false, ValidateServerCertificate);
                SslStream.AuthenticateAsClient(ServerName);
                EnsureDataReceiverIsRunning(client);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Method that is triggered when server acceptes a new client
        /// Authenticate as server and start accepting new data
        /// </summary>
        public virtual bool OnConnectServer(EasyTcpClient client)
        {
            NetworkStream = new NetworkStream(client.BaseSocket);
            SslStream = new SslStream(NetworkStream, false);
            SslStream.BeginAuthenticateAsServer(Certificate, ar =>
            {
                SslStream.EndAuthenticateAsServer(ar);
                EnsureDataReceiverIsRunning(client);
            }, null);
            return true;
        }

        /// <summary>
        /// Dispose protocol, automatically called by client.Dispose and server.Dispose 
        /// </summary>
        public virtual void Dispose()
        {
            AcceptArgs?.Dispose();
            SslStream?.Dispose();
            NetworkStream?.Dispose();
        }

        /*
         * Methods that need to be implemented/can be overridden when extending this class
         */

        /// <summary>
        /// Size of the receive buffer 
        /// </summary>
        public abstract int BufferSize { get; protected set; }

        /// <summary>
        /// Offset of the receive buffer, where to start saving the received data
        /// Default is offset is 0
        /// </summary>
        public virtual int BufferOffset { get => 0; protected set { } }

        /// <summary>
        /// The maximum amount of bytes to receive in the receive buffer
        /// Default count is the bufferSize - BufferOffset
        /// </summary>
        public virtual int BufferCount { get => BufferSize - BufferOffset; protected set { } }

        /// <summary>
        /// Send message to remote host
        /// This method should trigger the OnDataSend event
        /// </summary>
        public abstract void SendMessage(EasyTcpClient client, params byte[][] messageData);

        /// <summary>
        /// Handle received data, this function should trigger the OnDataReceive event of the passed client
        /// </summary>
        /// <param name="data">received data</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client">client that received the data</param>
        public abstract Task OnDataReceive(byte[] data, int receivedBytes, EasyTcpClient client);

        /// <summary>
        /// Return new instance of protocol
        /// Used by the server to create copies of protocol for all connected clients.
        /// </summary>
        public abstract IEasyProtocol Clone();

        /*
         * Internal methods
         */

        /// <summary>
        /// Determines whether a certificate is valid
        /// this function is a callback used by the OnConnect function
        /// </summary>
        protected virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
            => AcceptInvalidCertificates || sslPolicyErrors == SslPolicyErrors.None;

        /// <summary>
        /// Process a disconnected client 
        /// </summary>
        protected virtual void HandleDisconnect(EasyTcpClient client)
        {
            client.FireOnDisconnect();
            client.Dispose();
        }

        /// <summary>
        /// Callback method that accepts new tcp connections
        /// </summary>
        protected virtual void OnConnectCallback(SocketAsyncEventArgs ar)
        {
            var server = ar.UserToken as EasyTcpServer;
            if (server?.BaseSocket == null || !server.IsRunning) return;

            try
            {
                var client = new EasyTcpClient(server.Protocol.Clone())
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
            catch (Exception ex) { server.FireOnError(ex); }
        }

        /// <summary>
        /// Callback method that handles receiving data and disconnects
        /// </summary>
        protected virtual async void OnReceiveCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client == null) return;
            IsListening = false;

            try
            {
                int receivedBytes = SslStream.EndRead(ar);
                if (receivedBytes != 0)
                {
                    try { await OnDataReceive(ReceiveBuffer, receivedBytes, client); }
                    catch(Exception e) { throw new Exception("Exception occured while handling received data", e); }

                    if (client.BaseSocket == null) HandleDisconnect(client); // If client is disposed by DataReceive
                    else EnsureDataReceiverIsRunning(client);
                }
                else HandleDisconnect(client);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException) // Client disconnected and threw an error 
                    HandleDisconnect(client);
                else if (client.BaseSocket != null) client.FireOnError(ex);
            }
        }
    }
}

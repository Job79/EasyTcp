using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EasyTcp3.Protocols;
namespace EasyTcp3.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// Abstract implementation of the tcp/ssl protocol
    /// </summary>
    public abstract class DefaultSslProtocol : IEasyTcpProtocol
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
        /// Determines whether the DataReceiver is started
        /// </summary>
        protected bool IsListening;

        /// <summary>
        /// Instance of SslStream,
        /// null for base protocol of server
        /// </summary>
        protected SslStream SslStream;

        /// <summary>
        /// Instance of network stream,
        /// null for base protocol of server
        /// </summary>
        protected NetworkStream NetworkStream;

        /// <summary>
        /// Certificate used by SslStream
        /// ignored if protocol is used by a client
        /// </summary>
        protected readonly X509Certificate Certificate;

        /// <summary>
        /// ServerName used by SslStream.AuthenticateAsClient
        /// ignored if protocol is used by a server
        /// </summary>
        protected readonly string ServerName;

        /// <summary>
        /// Determines whether the client accepts servers with invalid certificates
        /// ignored if protocol is used by a server
        /// </summary>
        protected readonly bool AcceptInvalidCertificates;

        /// <summary>
        /// Constructor for servers
        /// </summary>
        /// <param name="certificate">server certificate</param>
        public DefaultSslProtocol(X509Certificate certificate) => Certificate = certificate;

        /// <summary>
        /// Constructor for clients
        /// </summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public DefaultSslProtocol(string serverName, bool acceptInvalidCertificates = false)
        {
            ServerName = serverName;
            AcceptInvalidCertificates = acceptInvalidCertificates;
        }

        /// <summary>
        /// Get new instance of a tcp socket 
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of socket compatible with the tcp protocol</returns>
        public virtual Socket GetSocket(AddressFamily addressFamily) =>
            new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Get receiving/sending stream
        /// TODO write tests
        /// </summary>
        /// <returns></returns>
        public Stream GetStream(EasyTcpClient client) => SslStream;

        /// <summary>
        /// Start accepting new clients
        /// </summary>
        /// <param name="server"></param>
        public virtual void StartAcceptingClients(EasyTcpServer server)
        {
            if (AcceptArgs == null)
            {
                server.BaseSocket.Listen(50000);
                AcceptArgs = new SocketAsyncEventArgs { UserToken = server };
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

            var protocol = (DefaultSslProtocol)client.Protocol;
            ((DefaultSslProtocol)client.Protocol).SslStream.BeginRead(
                protocol.ReceiveBuffer = new byte[BufferSize], 0, protocol.ReceiveBuffer.Length, OnReceiveCallback, client);
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
            SslStream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Method that is triggered when client connects to remote endpoint 
        /// Authenticate as client and start accepting new data
        /// </summary>
        /// <param name="client"></param>
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
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that is triggered when server accepted a new client
        /// Authenticate as server and start accepting new data
        /// </summary>
        /// <param name="client"></param>
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
            SslStream?.Dispose();
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
        /// Determines whether a certificate is valid
        /// this function is a callback used by the OnConnect function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        protected virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
            => AcceptInvalidCertificates || sslPolicyErrors == SslPolicyErrors.None;

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
                var client = new EasyTcpClient((IEasyTcpProtocol)server.Protocol.Clone())
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
                int receivedBytes = SslStream.EndRead(ar);
                if (receivedBytes != 0)
                {
                    await DataReceive(ReceiveBuffer, receivedBytes, client);

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
                else if (client.BaseSocket != null) client.FireOnError(ex);
            }
        }
    }
}

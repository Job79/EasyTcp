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
    /// Implementation of tcp protocol with ssl
    /// </summary>
    public abstract class DefaultSslProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Buffer with received data
        /// </summary>
        public byte[] ReceiveBuffer;
        
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
        /// null if protocol is used by client
        /// </summary>
        protected readonly X509Certificate Certificate;

        /// <summary>
        /// ServerName used by SslStream.AuthenticateAsClient
        /// null if protocol is used by server
        /// </summary>
        protected readonly string ServerName;

        /// <summary>
        /// Determines whether the client accepts servers with invalid certificates
        /// null if protocol is used by server
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
        /// Default socket for protocol
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of socket compatible with protocol</returns>
        public virtual Socket GetSocket(AddressFamily addressFamily) =>
            new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        /// <summary>
        /// Get receiving/sending stream
        /// </summary>
        /// <returns></returns>
        public Stream GetStream(EasyTcpClient client) => NetworkStream; 

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

            var protocol = (DefaultSslProtocol)client.Protocol;
            ((DefaultSslProtocol) client.Protocol).SslStream.BeginRead(protocol.ReceiveBuffer = new byte[BufferSize], 0,
                protocol.ReceiveBuffer.Length, OnReceiveCallback, client);
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
            SslStream.BeginWrite(message, 0, message.Length, ar =>
            {
                var stream = ar.AsyncState as SslStream;
                stream?.EndWrite(ar);
            }, SslStream);
        }

        /// <summary>
        /// Create new instance of current protocol,
        /// used by server when accepting a new client
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Dispose instance of sslStream and networkStream
        /// </summary>
        public virtual void Dispose()
        {
            SslStream?.Dispose();
            NetworkStream?.Dispose();
        }

        /// <summary>
        /// Method that is triggered when client connects
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
        /// Method that is triggered when client connects to server
        ///  Authenticate as server and start accepting new data
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
        /// Determines whether a certificate is valid
        /// this function is a callback used by OnConnect
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
                    var protocol = (DefaultSslProtocol)client.Protocol;
                    await DataReceive(protocol.ReceiveBuffer, receivedBytes, client);
                    
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
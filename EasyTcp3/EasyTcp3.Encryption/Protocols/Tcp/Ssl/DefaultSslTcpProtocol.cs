using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using EasyTcp3;
using EasyTcp3.Protocols;
using EasyTcp3.Server;

namespace EasyTcp.Encryption.Protocols.Tcp.Ssl
{
    /// <summary>
    /// Implementation of tcp protocol with ssl
    /// </summary>
    public abstract class DefaultSslTcpProtocol : IEasyTcpProtocol
    {
        /// <summary>
        /// Instance of SslStream
        /// </summary>
        protected SslStream SslStream;

        /// <summary>
        /// Instance of network stream
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
        /// Constructor if used by a server
        /// </summary>
        /// <param name="certificate">server certificate</param>
        public DefaultSslTcpProtocol(X509Certificate certificate) => Certificate = certificate;

        /// <summary>
        /// Constructor if used by a client
        /// </summary>
        /// <param name="serverName">domain name of server, must be the same as in server certificate</param>
        /// <param name="acceptInvalidCertificates">determines whether the client accepts servers with invalid certificates</param>
        public DefaultSslTcpProtocol(string serverName, bool acceptInvalidCertificates = false)
        {
            ServerName = serverName;
            AcceptInvalidCertificates = acceptInvalidCertificates;
        }

        /// <summary>
        /// Return new tcp socket
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of tcp socket</returns>
        public virtual Socket GetSocket(AddressFamily addressFamily) =>
            new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Call Listen() and start accepting new clients
        /// </summary>
        /// <param name="server"></param>
        public virtual void StartAcceptingClients(EasyTcpServer server)
        {
            server.BaseSocket.Listen(5000);
            server.BaseSocket.BeginAccept(OnConnectCallback, server);
        }

        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public virtual void StartDataReceiver(EasyTcpClient client)
            => ((DefaultSslTcpProtocol) client.Protocol).SslStream.BeginRead(client.Buffer = new byte[BufferSize], 0,
                client.Buffer.Length, OnReceiveCallback, client);

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// returned data will be send to remote host.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public abstract byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Send message to remote host 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <exception cref="Exception">could not send data: Client not connected or null</exception>
        public virtual void SendMessage(EasyTcpClient client, byte[] message)
        {
            if (client?.BaseSocket == null || !client.BaseSocket.Connected)
                throw new Exception("Could not send data: Client not connected or null");

            SslStream.BeginWrite(message, 0, message.Length, (ar) =>
                SslStream.EndWrite(ar), client);
        }

        /// <summary>
        /// Create new instance of current protocol,
        /// used by server when accepting a new client.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Dispose instances of sslStream, network stream and certificate if not null
        /// </summary>
        public virtual void Dispose()
        {
            SslStream?.Dispose();
            NetworkStream?.Dispose();
            Certificate?.Dispose();
        }

        /// <summary>
        /// Method that is triggered when client connects,
        /// Try to authenticate as client, return false if failed 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public virtual bool OnConnect(EasyTcpClient client)
        {
            try
            {
                NetworkStream = new NetworkStream(client.BaseSocket);
                SslStream = new SslStream(NetworkStream, false, ValidateServerCertificate);
                SslStream.AuthenticateAsClient(ServerName);
                StartDataReceiver(client);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that is triggered when client connects to server,
        /// try to authenticate as server 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public virtual bool OnConnectServer(EasyTcpClient client)
        {
            NetworkStream = new NetworkStream(client.BaseSocket);
            SslStream = new SslStream(NetworkStream, false);
            SslStream.BeginAuthenticateAsServer(Certificate, (ar) =>
            {
                SslStream.EndAuthenticateAsServer(ar);
                StartDataReceiver(client);
            }, null);
            return true;
        }

        /*
         * Methods used by internal receivers that need to be implemented when using this class 
         */

        /// <summary>
        /// Size of (next) buffer used by internal receive event 
        /// </summary>
        public abstract int BufferSize { get; protected set; }

        /// <summary>
        /// Function that handles received data.
        /// This function should call <code>client.DataReceiveHandler({Received message});</code> to trigger the OnDataReceive event
        /// </summary>
        /// <param name="data">received data, has the size of the clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes, can be smaller than data</param>
        /// <param name="client"></param>
        public abstract void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client);

        /*
         * Internal methods
         */

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
        {
            if (AcceptInvalidCertificates) return true;
            else return AcceptInvalidCertificates && sslPolicyErrors == SslPolicyErrors.None;
        }

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
        /// Callback method that accepts new tcp connections (server)
        /// Fired when a new client connects
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
                client.OnDataReceive += (_, message) => server.FireOnDataReceive(message);
                client.OnDisconnect += (_, c) => server.FireOnDisconnect(c);
                client.OnError += (_, exception) => server.FireOnError(exception);
                server.BaseSocket.BeginAccept(OnConnectCallback, server);

                if (!client.Protocol.OnConnectServer(client)) return;
                server.FireOnConnect(client);
                if (client.BaseSocket != null) //Check if user aborted OnConnect with Client.Dispose()
                {
                    lock (server.UnsafeConnectedClients) server.UnsafeConnectedClients.Add(client);
                }
            }
            catch (Exception ex)
            {
                server.FireOnError(ex);
            }
        }

        /// <summary>
        /// Method that handles receiving data (client & server)
        /// Fired when new data is received
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void OnReceiveCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as EasyTcpClient;
            if (client?.BaseSocket == null) return;

            try
            {
                if (!client.BaseSocket.Connected)
                {
                    HandleDisconnect(client);
                    return;
                }

                int receivedBytes = SslStream.EndRead(ar);
                if (receivedBytes != 0)
                {
                    DataReceive(client.Buffer, receivedBytes, client);
                    if (client.BaseSocket == null)
                        HandleDisconnect(client); // Check if client is disposed by DataReceive
                    else StartDataReceiver(client);
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
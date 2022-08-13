using System;
using System.IO;
using System.Net.Sockets;

namespace EasyTcp4.Protocols
{
    /// <summary>
    /// Template for EasyTcp protocols,
    /// A protocol determines all behavior of an EasyTcpClient and EasyTcpServer
    /// All protocol classes should implement IDisposable
    /// See the implemented protocols inside the Tcp folder for examples
    ///
    /// Feel free to open a pull request for any implemented protocol
    /// </summary>
    public interface IEasyProtocol : IDisposable
    {
        /// <summary>
        /// Get a new instance of a socket compatible with protocol 
        /// Used by the Connect and Start utils
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of a socket compatible with protocol</returns>
        public Socket GetSocket(AddressFamily addressFamily);

        /// <summary>
        /// Get receiving/sending stream
        /// Stream will not be disposed after use, reuse same stream and dispose stream in .Dispose function of protocol
        /// </summary>
        public Stream GetStream(EasyTcpClient client);

        /// <summary>
        /// Start accepting new connections 
        /// </summary>
        public void StartAcceptingClients(EasyTcpServer server);

        /// <summary>
        /// Start or continue listerning for incoming data
        /// </summary>
        public void EnsureDataReceiverIsRunning(EasyTcpClient client);

        /// <summary>
        /// Method that is triggered when client connected to remote endpoint 
        /// </summary>
        public bool OnConnect(EasyTcpClient client);

        /// <summary>
        /// Method that is triggered when server acceptes a new client
        /// </summary>
        public bool OnConnectServer(EasyTcpClient client);

        /// <summary>
        /// Send message to remote host
        /// This method should trigger the OnDataSend event
        /// </summary>
        public void SendMessage(EasyTcpClient client, params byte[][] messageData);

        /// <summary>
        /// Return new instance of protocol
        /// Used by the server to create copies of protocol for all connected clients.
        /// </summary>
        public IEasyProtocol Clone();
    }
}

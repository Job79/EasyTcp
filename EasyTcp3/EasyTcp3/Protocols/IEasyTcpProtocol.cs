using System;
using System.IO;
using System.Net.Sockets;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// Template for EasyTcp protocols,
    /// A protocol determines all behavior of an EasyTcpClient and EasyTcpServer
    /// All protocol classes should implement IDisposable
    /// See the implemented protocols inside the Tcp folder for examples
    ///
    /// Feel free to open a pull request for any implemented protocol
    /// </summary>
    public interface IEasyTcpProtocol : IDisposable
    {
        /// <summary>
        /// Get a new instance of a socket compatible with protocol 
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of a socket compatible with protocol</returns>
        public Socket GetSocket(AddressFamily addressFamily);

        /// <summary>
        /// Get receiving/sending stream
        /// Stream will NOT be disposed, use 1 stream instance per client.
        /// </summary>
        /// <returns></returns>
        public Stream GetStream(EasyTcpClient client);

        /// <summary>
        /// Start accepting new clients
        /// Bind is already called.
        /// </summary>
        /// <param name="server"></param>
        public void StartAcceptingClients(EasyTcpServer server);

        /// <summary>
        /// Start/continue listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public void EnsureDataReceiverIsRunning(EasyTcpClient client);

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// Returned data will be send to remote host.
        /// Use this function for implementing framing.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        /// TODO remove function
        public byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Send message to remote host
        /// Trigger OnDataSend here.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public void SendMessage(EasyTcpClient client, byte[] message);
         
        /// <summary>
        /// Method that is triggered when client connects to remote endpoint 
        /// Start here with listening for incoming data.
        /// </summary>
        /// <param name="client"></param>
        public bool OnConnect(EasyTcpClient client);

        /// <summary>
        /// Method that is triggered when server accepted a new client
        /// Start here with listening for incoming data.
        /// </summary>
        /// <param name="client"></param>
        public bool OnConnectServer(EasyTcpClient client);
        
        /// <summary>
        /// Return new instance of protocol
        /// Used by the server to create copies for all connected clients.
        /// </summary>
        /// <returns>new object</returns>
        public object Clone();
    }
}

using System;
using System.IO;
using System.Net.Sockets;
using EasyTcp3.Server;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// Interface for EasyTcp protocols,
    /// A protocol determines all behavior of an EasyTcpClient
    /// Protocol classes should also implement ICloneable and IDisposable
    /// ICloneable is used by the server to give every client an unique copy of protocol
    /// See implemented protocols for examples
    ///
    /// Feel free to open a pull request for any implemented protocol
    /// </summary>
    public interface IEasyTcpProtocol : ICloneable, IDisposable
    {
        /// <summary>
        /// Default socket for protocol
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <returns>new instance of socket compatible with protocol</returns>
        public Socket GetSocket(AddressFamily addressFamily);

        /// <summary>
        /// Start accepting new clients
        /// Bind is already called
        /// </summary>
        /// <param name="server"></param>
        public void StartAcceptingClients(EasyTcpServer server);

        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public void EnsureDataReceiverIsRunning(EasyTcpClient client);

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// returned data will be send to remote host
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Send message to remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public void SendMessage(EasyTcpClient client, byte[] message);
        
        /*
         * Optional 
         */

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
    }
}
using System;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Protocols
{
    /// <summary>
    /// Interface for a custom protocol,
    /// determines behavior when connecting, receiving and sending data.
    /// Protocol classes should also implement <code>object Clone();</code>. This should return a new Protocol object. (Used by the server for new connected clients)
    /// See implemented protocols for examples.
    ///
    /// Feel free to open a pull request for any implemented protocol.
    /// </summary>
    public interface IEasyTcpProtocol : ICloneable
    {
        /// <summary>
        /// Variable is used after receiving data,
        /// size of new buffer (and thus size of next receiving data)
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Method that creates a message from a byte[] or byte[][]
        /// returned data will be send to remote host.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Function that handles received data.
        /// This function should call <code>client.DataReceiveHandler({Received message});</code> to trigger the OnDataReceive event
        /// </summary>
        /// <param name="data">received data, has the size of the clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client);

        /// <summary>
        /// Method that is triggered when client connects.
        /// ! Triggered before OnConnect event
        /// Default behavior is starting listening for incoming data.
        /// This method should call <code>client.StartInternalDataReceiver();</code>
        /// </summary>
        /// <param name="client"></param>
        public void OnConnect(EasyTcpClient client) => client.StartInternalDataReceiver();

        /// <summary>
        /// Method that is triggered when client connects to server
        /// ! Triggered before OnConnect event
        /// ! Blocks accepting new sockets
        /// Default behavior is starting listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public void OnConnectServer(EasyTcpClient client) => client.StartInternalDataReceiver();
    }
}
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Protocol
{
    /// <summary>
    /// Interface for a custom protocol,
    /// see DefaultProtocol for an example
    /// </summary>
    public interface IEasyTcpProtocol
    {
        /// <summary>
        /// Method that creates a message from a byte[] or byte[][]
        /// returned data will be send to remote host.
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public byte[] CreateMessage(params byte[][] data);

        /// <summary>
        /// Variable is used after receiving data,
        /// size of buffer (and size of next receiving data)
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// Function that is triggered when new data is received.
        /// </summary>
        /// <param name="data">received data, has the size of the client buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        /// <returns>arrays or array with received data, for each element 1 event is triggered.</returns>
        public void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client) => client.FireOnDataReceiveEvent(new Message(data, client));

        /// <summary>
        /// Method that is triggered when client connects
        /// Default behavior is starting listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public void OnConnect(EasyTcpClient client) => client.StartInternalDataReceiver();

        /// <summary>
        /// Method that is triggered when client connects to server
        /// Default behavior is starting listening for incoming data
        /// </summary>
        /// <param name="client"></param>
        public void OnConnectServer(EasyTcpClient client) => client.StartInternalDataReceiver();
    }
}
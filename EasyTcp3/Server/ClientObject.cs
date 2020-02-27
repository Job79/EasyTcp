using System.Net.Sockets;

namespace EasyTcp3.Server
{
    internal class ClientObject
    {
        //Client socket.
        public Socket Socket;

        /// <summary>
        /// Data buffer for incoming data.
        /// </summary>
        public byte[] Buffer;
        
        public bool ReceiveData;

        public bool IsConnected()
            => Socket.Poll(0, SelectMode.SelectRead) && Socket.Available.Equals(0);
    }
}
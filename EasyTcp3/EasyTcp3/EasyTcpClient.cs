using System;
using System.Net.Sockets;

namespace EasyTcp3
{
    public class EasyTcpClient : IDisposable
    {
        public Socket BaseSocket { get; protected internal set; }
        
        protected internal bool ReceivingData;
        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;
        protected internal void FireOnConnect() => OnConnect?.Invoke(null, this);

        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;
        protected internal void FireOnDisconnect() => OnDisconnect?.Invoke(null, this);
        
        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<Message> OnDataReceive;
        protected internal void FireOnDataReceive(Message e) => OnDataReceive?.Invoke(this, e);
        
        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;
        protected internal void FireOnError(Exception e)
        {
            if (OnError != null) OnError.Invoke(this, e);
            else throw e;
        }

        /// <summary>
        /// Buffer used for receiving incoming data
        /// </summary>
        protected internal byte[] Buffer;

        /// <summary>
        /// Dispose current instance of the baseSocket
        /// </summary>
        public void Dispose()
        {
            BaseSocket?.Dispose();
            BaseSocket = null;
        }

        public EasyTcpClient() { }
        public EasyTcpClient(Socket socket) : this() => BaseSocket = socket;
    }
}
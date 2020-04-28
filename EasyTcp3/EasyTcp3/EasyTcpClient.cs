using System;
using System.Net.Sockets;

namespace EasyTcp3
{
    //TODO: Add async functions (SendAndGetReply, Connect, Maybe send)
    //TODO: Add serialisation of custom classes
    //TODO: Add receive stream in Message and SendStream
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of this Client,
        /// Gets disposed when calling Dispose()
        /// Null if disconnected
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }
        
        /// <summary>
        /// Determines whether the next receiving data is the length of data or actual data. [Length of data (4)] ["Data"] 
        /// See OnReceive for more information about the protocol
        /// </summary>
        protected internal bool ReceivingData;
        
        /// <summary>
        /// Buffer used for receiving incoming data
        /// </summary>
        protected internal byte[] Buffer = new byte[2];
        
        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;
        
        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;
        
        /// <summary>
        /// Fired when a client connects to the server
        /// </summary>
        public event EventHandler<Message> OnDataReceive;
        
        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;
        
        protected internal void FireOnConnect() => OnConnect?.Invoke(null, this);
        protected internal void FireOnDisconnect() => OnDisconnect?.Invoke(null, this);
        protected internal void FireOnDataReceive(Message e) => OnDataReceive?.Invoke(this, e);
        protected internal void FireOnError(Exception e)
        {
            if (OnError != null) OnError.Invoke(this, e);
            else throw e;
        }
        
        public EasyTcpClient() { }
        public EasyTcpClient(Socket socket) : this() => BaseSocket = socket;

        /// <summary>
        /// Dispose current instance of the baseSocket if not null
        /// </summary>
        public void Dispose()
        {
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}
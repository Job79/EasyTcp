using System;
using System.Net.Sockets;

namespace EasyTcp3
{
    /// <summary>
    /// Class that holds all the information and some functions of an EasyTcpClient
    /// See ClientUtils for more functions
    /// </summary>
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of client,
        /// Gets disposed when calling Dispose()
        /// Null if disconnected
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }
        
        /// <summary>
        /// Determines whether the next receiving data is the length of data or actual data. [Length of data (4)] ["Data"] 
        /// See OnReceive for more information about the protocol EasyTcp uses
        /// </summary>
        protected internal bool ReceivingData;
        
        /// <summary>
        /// Buffer used for receiving incoming data. See Internal/OnConnectUtil.cs for usage
        /// </summary>
        protected internal byte[] Buffer = new byte[2];
        
        /// <summary>
        /// Fired when the client connects to a server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;
        
        /// <summary>
        /// Fired when a client disconnects from a server to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;
        
        /// <summary>
        /// Fired when a client receives new information,
        /// Doesn't fire when one of these functions are called:
        /// SendAndGetReply
        /// SendAndReplyAsync
        /// </summary>
        public event EventHandler<Message> OnDataReceive;
        
        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;
        
        /// <summary>
        /// Function used to fire the OnConnect event
        /// </summary>
        protected internal void FireOnConnect() => OnConnect?.Invoke(this, this);
        /// <summary>
        /// Function used to fire the OnDisconnect event
        /// </summary>
        protected internal void FireOnDisconnect() => OnDisconnect?.Invoke(this, this);
        /// <summary>
        /// Function used to fire the OnDataReceive event
        /// </summary>
        /// <param name="message"></param>
        protected internal void FireOnDataReceiveEvent(Message message) => OnDataReceive?.Invoke(this, message);
        protected internal Action<Message> FireOnDataReceive;
        /// <summary>
        /// Function used to fire the OnError event,
        /// or if event is null, throw an exception
        /// </summary>
        /// <param name="exception"></param>
        protected internal void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        public EasyTcpClient()
        {
            FireOnDataReceive = FireOnDataReceiveEvent;
        }
        public EasyTcpClient(Socket socket) : this() => BaseSocket = socket;

        /// <summary>
        /// Dispose current instance of the baseSocket if not null
        /// Client will disconnect when function is called
        /// </summary>
        public void Dispose()
        {
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}
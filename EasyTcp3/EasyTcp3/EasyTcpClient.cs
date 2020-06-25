using System;
using System.Net.Sockets;
using EasyTcp3.Protocols;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3
{
    /// <summary>
    /// EasyTcp client,
    /// Provides a simple high performance tcp client
    /// </summary>
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of client,
        /// null if client is not connected to remote host
        /// </summary>
        public Socket BaseSocket { get; set; }

        /// <summary>
        /// Protocol for client, protocol determines all behavior of this client
        /// </summary>
        public IEasyTcpProtocol Protocol
        {
            get => _protocol;
            set
            {
                if (BaseSocket != null) throw new Exception("Can not change protocol when client is connected");
                _protocol = value;
            }
        }

        private IEasyTcpProtocol _protocol;

        /// <summary>
        /// Function used by send functions to Serialize objects
        /// </summary>
        public Func<object, byte[]> Serialize = o =>
            throw new Exception("Assign a function to serialize first before using serialisation");

        /// <summary>
        /// Function used by receive to Deserialize byte[] to object 
        /// </summary>
        public Func<byte[], Type, object> Deserialize = (b, t) =>
            throw new Exception("Assign a function to deserialize first before using serialisation");

        /// <summary>
        /// Buffer with received data
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// Event that is fired when client connected to remote host 
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Event that is fired when client disconnects from remote host
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Event that is fired when client receives data from remote host
        /// </summary>
        public event EventHandler<Message> OnDataReceive;

        /// <summary>
        /// Event that is fired when error occurs
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Fire the OnConnect event
        /// </summary>
        public void FireOnConnect() => OnConnect?.Invoke(this, this);

        /// <summary>
        /// Fire the OnDisconnect event
        /// </summary>
        public void FireOnDisconnect() => OnDisconnect?.Invoke(this, this);

        /// <summary>
        /// Fire the OnError event,
        /// throw error if event is not used and library is compiled with debug mode
        /// </summary>
        /// <param name="exception"></param>
        public void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        /// <summary>
        /// Fire the OnDataReceive event
        /// </summary>
        /// <param name="message">received message</param>
        public void FireOnDataReceiveEvent(Message message) => OnDataReceive?.Invoke(this, message);

        /// <summary>
        /// Execute custom action when receiving data 
        /// </summary>
        public Action<Message> DataReceiveHandler;

        /// <summary>
        /// Set DataReceiveHandler back to default behavior (calling OnDataReceive)
        /// </summary>
        public void ResetDataReceiveHandler() => DataReceiveHandler = FireOnDataReceiveEvent;

        /// <summary></summary>
        /// <param name="protocol"></param>
        public EasyTcpClient(IEasyTcpProtocol protocol = null)
        {
            Protocol = protocol ?? new PrefixLengthProtocol();
            ResetDataReceiveHandler();
        }

        /// <summary></summary>
        /// <param name="socket"></param>
        /// <param name="protocol"></param>
        public EasyTcpClient(Socket socket, IEasyTcpProtocol protocol = null) : this(protocol) => BaseSocket = socket;

        /// <summary>
        /// Dispose current instance of the baseSocket if not null
        /// </summary>
        public void Dispose()
        {
            Protocol?.Dispose();
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp3.Protocols;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3
{
    /// <summary>
    /// EasyTcp client
    /// Class with all EasyTcpClient properties and basic functions.
    /// See the "ClientUtils" classes for all the other functions.
    /// </summary>
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of client
        /// null if client is not connected to remote host.
        /// </summary>
        public Socket BaseSocket { get; set; }

        /// <summary>
        /// Protocol used for this connection
        /// The used protocol determines the internal behavior of the client. 
        /// The protocol can't be changed when connected. 
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
        /// List with session variables
        /// Available to store custom information.
        /// </summary>
        public Dictionary<string, object> Session
        {
            get => _session ??= new Dictionary<string, object>();
            set => _session = value;
        }
        
        private Dictionary<string, object> _session;

        /// <summary>
        /// Function used by send functions to Serialize custom objects
        /// </summary>
        public Func<object, byte[]> Serialize = o =>
            throw new Exception("Assign a function to serialize first before using serialisation");

        /// <summary>
        /// Function used by receive to Deserialize byte[] to custom object 
        /// </summary>
        public Func<byte[], Type, object> Deserialize = (b, t) =>
            throw new Exception("Assign a function to deserialize first before using serialisation");

        /// <summary>
        /// Event that is fired when client connects to remote host 
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
        /// Async event that is fired when client receives data from remote host
        /// </summary>
        public event OnDataReceiveAsyncDelegate OnDataReceiveAsync;
        
        /// <summary>
        /// Delegate type for OnDataReceiveAsync
        /// </summary>
        public delegate Task OnDataReceiveAsyncDelegate(object sender, Message message);
        
        /// <summary>
        /// Event that is fired when client sends any data to remote host
        /// </summary>
        public event EventHandler<Message> OnDataSend;

        /// <summary>
        /// Event that is fired when an (internal) error occurs
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
        /// Fire the OnDataSend event
        /// </summary>
        /// <param name="data"></param>
        public void FireOnDataSend(byte[] data) => OnDataSend?.Invoke(this, new Message(data, this));

        /// <summary>
        /// Fire the OnError event,
        /// throw error if event handler isn't used
        /// </summary>
        /// <param name="exception"></param>
        public void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        /// <summary>
        /// Execute custom action when receiving data 
        /// </summary>
        public Func<Message, Task> DataReceiveHandler;
        
        /// <summary>
        /// Fire the OnDataReceive & OnDataReceiveAsync events
        /// </summary>
        /// <param name="message">received message</param>
        private async Task FireOnDataReceiveEvent(Message message)
        {
            if (OnDataReceiveAsync != null) await OnDataReceiveAsync.Invoke(this, message);
            OnDataReceive?.Invoke(this, message);
        } 

        /// <summary>
        /// Set DataReceiveHandler back to default behavior (calling private function FireOnDataReceiveEvent)
        /// </summary>
        public void ResetDataReceiveHandler() => DataReceiveHandler = FireOnDataReceiveEvent;

        /// <summary>
        /// Construct new EasyTcpClient
        /// </summary>
        /// <param name="protocol"></param>
        public EasyTcpClient(IEasyTcpProtocol protocol = null)
        {
            Protocol = protocol ?? new PrefixLengthProtocol();
            ResetDataReceiveHandler(); // Set DataReceiveHandler to default behavior
#if (NETCOREAPP3_1 || NET5_0)
            Serialize = o => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(o);
            Deserialize = (b, t) => System.Text.Json.JsonSerializer.Deserialize(b, t);
#endif
        }

        /// <summary>
        /// Dispose and disconnect EasyTcpClient
        /// </summary>
        public void Dispose()
        {
            Protocol?.Dispose();
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}

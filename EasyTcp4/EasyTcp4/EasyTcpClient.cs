using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyTcp4.Protocols;
using EasyTcp4.Protocols.Tcp;

namespace EasyTcp4
{
    /// <summary>
    /// Class with all EasyTcpClient properties and basic functions
    /// See the "ClientUtils" folder for all the other functions.
    /// </summary>
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of client
        /// null if client isn't connected to remote host.
        /// </summary>
        public Socket BaseSocket { get; set; }

        /// <summary>
        /// Protocol used for this connection
        /// The used protocol determines the internal behavior of the client. 
        /// The protocol can't be changed when connected. 
        /// </summary>
        public IEasyProtocol Protocol
        {
            get => _protocol;
            set
            {
                if (BaseSocket != null) throw new Exception("Can't change protocol when client is connected");
                _protocol = value;
            }
        }

        private IEasyProtocol _protocol;

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
        /// Function used to serialize custom objects
        /// </summary>
        public Func<object, byte[]> Serialize = o =>
            throw new Exception("No serialize function specified");

        /// <summary>
        /// Function used to deserialize byte[] to custom objects
        /// </summary>
        public Func<byte[], Type, object> Deserialize = (b, t) =>
            throw new Exception("No deserialize function specified");

        /// <summary>
        /// Event that is fired when client connects to remote host 
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Event that is fired when client disconnects from remote host
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Event that is fired when client sends any data to remote host
        /// </summary>
        public event EventHandler<Message> OnDataSend;

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
        public void FireOnDataSend(byte[] message, int offset = 0, int count = int.MaxValue)
        {
            if (OnDataSend == null) return;

            if (offset == 0 && count >= message.Length) OnDataSend.Invoke(this, new Message(message, this));
            else
            {
                byte[] data = new byte[Math.Min(count, message.Length) - offset];
                Buffer.BlockCopy(message, offset, data, 0, Math.Min(message.Length - offset, count));
                OnDataSend.Invoke(this, new Message(data, this));
            }
        }

        /// <summary>
        /// Execute custom action when receiving data 
        /// </summary>
        public Func<Message, Task> DataReceiveHandler;

        /// <summary>
        /// Fire the OnDataReceive and OnDataReceiveAsync events
        /// </summary>
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
        /// Fire the OnError event,
        /// throw error if event handler isn't used
        /// </summary>
        public void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
            else throw exception;
        }

        /// <summary>
        /// Construct new EasyTcpClient
        /// </summary>
        public EasyTcpClient(IEasyProtocol protocol = null)
        {
            Protocol = protocol ?? new PrefixLengthProtocol();
            ResetDataReceiveHandler(); // Set DataReceiveHandler to default behavior
#if (NETCOREAPP3_1 || NET5_0 || NET6_0)
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

using System;
using System.Net.Sockets;

namespace EasyTcp3
{
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of this Client,
        /// Gets disposed when calling Dispose()
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }

        /// <summary>
        /// Determines whether the next receiving data is length of data or actual data.
        /// See OnReceive for the protocol
        /// </summary>
        protected internal bool ReceivingData;
        
        /// <summary>
        /// Buffer used for receiving incoming data
        /// </summary>
        protected internal byte[] Buffer;

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
        ///
        /// <example>
        /// // Example receiving for the server
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///
        /// const string message = "Hello server!";
        /// int receiveCount = 0;
        ///
        /// server.OnConnect += (sender, client) =>
        /// {
        ///     client.OnDataReceive += (sender, receivedMessage) =>
        ///     {
        ///         //Async lambda, thread safe increase integer
        ///         if (message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount);
        ///         Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
        ///     };
        ///     Console.WriteLine("Client connected");
        /// };
        ///
        /// using var client = new EasyTcpClient();
        /// Assert.IsTrue(client.Connect(IPAddress.Any, port));
        /// client.Send(message);
        ///
        /// TestHelper.WaitWhileTrue(() => receiveCount == 0);
        /// Assert.AreEqual(1, receiveCount);
        /// </example>
        ///
        /// <example>
        /// //Example receiving for the client
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        /// 
        /// using var client = new EasyTcpClient();
        /// Assert.IsTrue(client.Connect(IPAddress.Any, port));
        ///
        /// const string message = "Hello server!";
        /// int receiveCount = 0;
        ///
        /// client.OnDataReceive += (sender, receivedMessage) =>
        /// {
        ///     //Async lambda, thread safe increase integer
        ///     if(message.Equals(receivedMessage.ToString())) Interlocked.Increment(ref receiveCount); 
        ///     Console.WriteLine($"[{receiveCount}]Received message: {receivedMessage.ToString()}");
        /// };
        ///    
        /// server.SendAll(message);
        ///
        /// TestHelper.WaitWhileTrue(() => receiveCount == 0);
        /// Assert.AreEqual(1, receiveCount);
        /// </example>
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
        /// Dispose current instance of the baseSocket
        /// </summary>
        public void Dispose()
        {
            BaseSocket?.Dispose();
            BaseSocket = null;
        }

        public EasyTcpClient()
        {
        }

        public EasyTcpClient(Socket socket) : this() => BaseSocket = socket;
    }
}
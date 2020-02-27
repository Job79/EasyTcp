using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace EasyTcp3.Client
{
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// Client socket
        /// </summary>
        public Socket BaseSocket { get; protected set; }
        
        /// <summary>
        /// Receive buffer
        /// </summary>
        private byte[] _buffer;

        public event EventHandler<Message> OnDataReceive;
        public event EventHandler<EasyTcpClient> OnConnect;
        public event EventHandler<EasyTcpClient> OnDisconnect;
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Encoding used for strings
        /// </summary>
        private Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentException("Encoding can't be set to null.");
        }
        
        /// <summary>
        /// Determines whether the next item to be received data or length
        /// </summary>
        private bool _receiveData;

        public EasyTcpClient()
        {
        }
        /// <summary>
        /// Create new EasyTcpClient with socket object
        /// ! All eventHandlers are useless until you connect
        /// </summary>
        /// <param name="socket"></param>
        public EasyTcpClient(Socket socket) => BaseSocket = socket;

        public bool Connect(IPAddress address, ushort port, TimeSpan timeout)
        {
            if (address == null) throw new ArgumentException("Could not connect: IP is null");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid Port");
            if(BaseSocket != null) throw new ArgumentException("Could not connect, disconnect first");

            try
            {
                BaseSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                BaseSocket.BeginConnect(address, port, null, null).AsyncWaitHandle.WaitOne(timeout);
                if (BaseSocket.Connected)
                {
                    OnConnect?.Invoke(this, this);
                    _buffer = new byte[2]; //Start listening for data
                    BaseSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, BaseSocket);
                    return true;
                }
            }
            catch { }

            Dispose();
            return false;
        }

        public bool IsConnected(bool fastCheck = false)
        {
            if (BaseSocket == null) return false;
            if (!BaseSocket.Connected || !fastCheck &&
                BaseSocket.Poll(0, SelectMode.SelectRead) &&
                BaseSocket.Available.Equals(0))
            {
                HandleDisconnect();
                return false;
            }
            else return true;
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (!IsConnected(false)) return;
            //ToDo: Test if fastCheck is enough

            try
            {
                ushort dataLength = 2;
                if (_receiveData = !_receiveData) //If receiving length
                {
                    if ((dataLength = BitConverter.ToUInt16(_buffer, 0)) == 0)
                    {
                        HandleDisconnect();
                        return;
                    }
                }
                else OnDataReceive?.Invoke(this, new Message(_buffer, this)); //Trigger event

                BaseSocket.BeginReceive(_buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
                    OnReceive, BaseSocket); //Start accepting the data.
            }
            catch (SocketException)
            { HandleDisconnect(); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        private void HandleDisconnect()
        {
            if(BaseSocket == null) return;
            OnDisconnect?.Invoke(this, this);
            Dispose();
        }
        
        public void Dispose()
        {
            if (BaseSocket == null) return;
            BaseSocket.Dispose();
            BaseSocket = null;
        }

        /*This function is used to handle errors*/
        private void NotifyOnError(Exception ex)
        {
            if (OnError != null) OnError(this, ex);
            else throw ex;
        }
    }
}
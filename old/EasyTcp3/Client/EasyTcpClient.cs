using System;
using System.Net;
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
        public byte[] Buffer;

        public event EventHandler<Message> OnDataReceive;
        public event EventHandler<EasyTcpClient> OnConnect;
        public event EventHandler<EasyTcpClient> OnDisconnect;
        public event EventHandler<Exception> OnError;
        
        
        /// <summary>
        /// Determines whether the next item to be received data or length
        /// </summary>
        public bool ReceiveData;

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
                    Buffer = new byte[2]; //Start listening for data
                    BaseSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceive, BaseSocket);
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
                if (ReceiveData = !ReceiveData) //If receiving length
                {
                    if ((dataLength = BitConverter.ToUInt16(Buffer, 0)) == 0)
                    {
                        HandleDisconnect();
                        return;
                    }
                }
                else OnDataReceive?.Invoke(this, new Message(Buffer, this)); //Trigger event

                BaseSocket.BeginReceive(Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
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
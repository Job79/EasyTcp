using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace EasyTcp3.Client
{
    public class EasyTcpClient : IDisposable
    {
        public Socket BaseSocket { get; protected set; }
        private byte[] _buffer;

        public event EventHandler<Message> OnDataReceive;
        public event EventHandler<EasyTcpClient> OnConnect;
        public event EventHandler<EasyTcpClient> OnDisconnect;
        public event EventHandler<Exception> OnError;

        private Encoding _encoding = Encoding.UTF8;

        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentException("Encoding can't be set to null.");
        }

        public bool Connect(IPAddress address, ushort port, TimeSpan timeout)
        {
            if (address == null) throw new ArgumentException("Could not connect: IP is null.");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid Port.");

            try
            {
                BaseSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                BaseSocket.BeginConnect(address, port, null, null).AsyncWaitHandle.WaitOne(timeout);
                if (BaseSocket.Connected)
                {
                    OnConnect?.Invoke(this, this);
                    _buffer = new byte[2]; //Start listening for data
                    BaseSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
                    return true;
                }
            }
            catch
            {
            }

            Dispose();
            return false;
        }

        public bool IsConnected(bool fastCheck = false)
        {
            if (BaseSocket == null) return false;
            else if (!BaseSocket.Connected || !fastCheck &&
                BaseSocket.Poll(0, SelectMode.SelectRead) &&
                BaseSocket.Available.Equals(0))
            {
                Dispose();
                return false;
            }
            else return true;
        }

        public void Dispose()
        {
            if (BaseSocket == null) return;
            BaseSocket.Dispose();
            BaseSocket = null;
        }

        public void Send(byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Could not send data: Data is empty.");
            if (IsConnected(true)) throw new Exception("Could not send data: Socket not connected.");

            var message = new byte[2 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length),
                0, message, 0, 2); //Write length of data to message.
            Buffer.BlockCopy(data, 0, message, 2, data.Length); //Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            BaseSocket.SendAsync(e);
        }

        private bool receiveData = false;
        private void OnReceive(IAsyncResult ar)
        {
            if (!IsConnected(true)) return; //ToDo: Test if fastCheck is enough

            try
            {
                ushort dataLength = 2;
                if (receiveData = !receiveData) //If receiving length
                {
                    if ((dataLength = BitConverter.ToUInt16(_buffer, 0)) == 0)
                    {
                        Dispose();
                        OnDisconnect?.Invoke(this, this);
                    }
                }
                else OnDataReceive?.Invoke(this, new Message(_buffer)); //Trigger event

                BaseSocket.BeginReceive(_buffer = new byte[dataLength], 0, dataLength, SocketFlags.None,
                    OnReceive, null); //Start accepting the data.
            }
            catch (SocketException)
            {
                Dispose();
                OnDisconnect?.Invoke(this, this);
            }
            catch (Exception ex)
            {
                NotifyOnError(ex);
            }
        }

        /*This function is used to handle errors*/
        private void NotifyOnError(Exception ex)
        {
            if (OnError != null) OnError(this, ex);
            else throw ex;
        }

        /* Helper methods:
         
                 public void Disconnect(bool notifyOnDisconnect = true)
        {
            if (!IsConnected()) return;
            Dispose();
            OnDisconnect?.Invoke(this, this); //Call OnDisconnect.
        }
         */
    }
}
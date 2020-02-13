using System;
using System.Net;
using System.Text;
using EasyTcp.Core;
using System.Threading;
using System.Net.Sockets;

namespace EasyTcp.Client
{
    public class EasyTcpClient
    {
        /// <summary>
        /// Raw client socket
        /// </summary>
        public Socket Socket { get; protected set; }

        /// <summary>
        /// DataReceived, triggered when a message is received and no other data is available.
        /// </summary>
        public event EventHandler<Message> DataReceived;

        /// <summary>
        /// OnDisconnect, triggered when a client disconnects from the server.
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// OnError, triggered when an error occurs.
        /// If not used error will be thrown.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Encoding to handle strings
        /// Used for sending and receiving data.
        /// Default: UTF8
        /// </summary>
        private Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentException("Encoding can't be set to null.");
        }

        /// <summary>
        /// Data buffer for incoming data.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Connect to server with ip as string
        /// </summary>
        /// <param name="ipString">IP address as string</param>
        /// <param name="port">Port as ushort(1-65 535)</param>
        /// <param name="timeout">Maximum timeout for connecting to server</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>>
        public bool Connect(string ipString, ushort port, TimeSpan timeout)
        {
            if (!IPAddress.TryParse(ipString, out IPAddress ip))
                throw new ArgumentException("Invalid IPv4/IPv6 address.");
            return Connect(ip, port, timeout);
        }
        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="ip">IP address as IPAddress</param>
        /// <param name="port">Port as ushort(1-65 535)</param>
        /// <param name="timeout">Maximum timeout for connecting to server</param>
        /// <returns>bool, true = Connected, false = failed to connect</returns>
        public bool Connect(IPAddress ip, ushort port, TimeSpan timeout)
        {
            if (ip == null) throw new ArgumentException("Could not connect: IP is null.");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid Port.");

            //Create socket.
            var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Try to connect.
            try
            {
                socket.BeginConnect(ip, port, null, null).AsyncWaitHandle.WaitOne(timeout);
            }
            catch//ToDo: fix duplicated code
            {
                socket.Close();
                return false;
            }

            if (!Socket.Connected)//Check if socket is connected or timeout is expired.
            {
                socket.Close();
                return false;
            }
          
            //Start listening for data.
            _buffer = new byte[2];
            Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceiveLength, null);
            return true;
        }

        /// <summary>
        /// Disconnect from server.
        /// </summary>
        /// <param name="notifyOnDisconnect">Calls the OnDisconnect handler if set to true, default true</param>
        public void Disconnect(bool notifyOnDisconnect = true)
        {
            if (Socket == null) return;
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket = null;
            }
            catch (Exception ex)
            {
                NotifyOnError(ex);
            }

            if (notifyOnDisconnect)
                OnDisconnect?.Invoke(this, this); //Call OnDisconnect.
        }

        //ToDo: Update documentation
        /// <summary>
        /// Return the current connection state of the client socket.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Socket == null || Socket.Poll(0, SelectMode.SelectRead) && Socket.Available.Equals(0))
                {
                    Disconnect(true);
                    return false;
                }
                else return true;
            }
        }

        /// <summary>
        /// Send data(short) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(short data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(int) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(int data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(long) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(long data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(double) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(double data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(float) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(float data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(bool) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(bool data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(char) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(char data)
            => Send(BitConverter.GetBytes(data));
        /// <summary>
        /// Send data(string) to server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        public void Send(string data)
            => Send(_encoding.GetBytes(data ?? throw new ArgumentException("Could not send data: Data is null.")));
        public void Send(byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Could not send data: Data is empty.");
            if (Socket == null) throw new Exception("Could not send data: Socket not connected.");

            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length), 0, message, 0,
                2); //Write length of data to message.
            Buffer.BlockCopy(data, 0, message, 2, data.Length); //Write data to message.

            using SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            Socket.SendAsync(e); //Write async so it won't block UI applications.
        }

        /// <summary>
        /// Send data(short) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(short data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(int) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(int data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(long) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(long data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(double) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(double data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(float) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(float data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(bool) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(bool data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(char) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(char data, TimeSpan timeout)
            => SendAndGetReply(BitConverter.GetBytes(data), timeout);
        /// <summary>
        /// Send data(string) to server. Then wait for a reply from the server.
        /// </summary>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public Message SendAndGetReply(string data, TimeSpan timeout)
            => SendAndGetReply(_encoding.GetBytes(data ?? throw new ArgumentException("Could not send data: Data is null.")), timeout);
        public Message SendAndGetReply(byte[] data, TimeSpan timeout)
        {
            Message reply = null;
            using ManualResetEventSlim signal = new ManualResetEventSlim();

            void Event(object sender, Message e)
            {
                reply = e;
                DataReceived -= Event;
                signal.Set(); // Method can't be triggered after signal is disposed.
            }

            DataReceived += Event;
            Send(data);

            signal.Wait(timeout);

            if (reply == null) DataReceived -= Event;
            return reply;
        }

        private void OnReceiveLength(IAsyncResult ar) //TODO: Check if poll can be removed an <= can be replaced with ==
        {
            if(!IsConnected) return;
            
            try
            {
                ushort dataLength = BitConverter.ToUInt16(_buffer, 0); //Get the length of the upcoming data.

                if (dataLength <= 0) Disconnect(true); //Invalid length, close connection. 
                else Socket.BeginReceive(_buffer = new byte[dataLength], 0, dataLength, SocketFlags.None, OnReceiveData, null); //Start accepting the data.
            }
            catch (SocketException) { Disconnect(true); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            if(!IsConnected) return;

            try
            {
                DataReceived?.Invoke(this, new Message(_buffer, Socket, _encoding)); //Trigger event
                Socket.BeginReceive(_buffer = new byte[2], 0, _buffer.Length, SocketFlags.None, OnReceiveLength, null); //Start receiving next message.
            }
            catch (SocketException){ Disconnect(true); }
            catch (Exception ex) { NotifyOnError(ex); }
        }

        /*This function is used to handle errors*/
        private void NotifyOnError(Exception ex)
        {
            if (OnError != null)
                OnError(this, ex);
            else throw ex;
        }
    }
}
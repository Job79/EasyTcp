using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

namespace HenkTcp
{
    internal class ServerListener
    {
        private List<TcpClient> _ConnectedClients = new List<TcpClient>();
        public List<TcpClient> ConnectedClients { get { return _ConnectedClients.ToList(); } }
        public void Stop() { Listener.Stop(); }

        public TcpListener Listener { get; }
        private readonly int _BufferSize;
        private readonly int _MaxConnections;
        private readonly HenkTcpServer _Parent;

        public ServerListener(TcpListener Listener, HenkTcpServer Parent, int MaxConnections, int BufferSize)
        {
            try
            {
                _Parent = Parent;
                _BufferSize = BufferSize;
                _MaxConnections = MaxConnections;

                //start the listener
                this.Listener = Listener;
                Listener.Start();
                Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        private void _OnClientConnect(IAsyncResult ar)
        {
            try
            {
                if (_ConnectedClients.Count >= _MaxConnections) { Listener.EndAcceptTcpClient(ar).Close(); Listener.BeginAcceptTcpClient(_OnClientConnect, Listener); }
                else
                {
                    ClientObject Client = new ClientObject();
                    Client.TcpClient = Listener.EndAcceptTcpClient(ar);
                    Client.Buffer = new byte[_BufferSize];

                    Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);

                    lock (_ConnectedClients) { _ConnectedClients.Add(Client.TcpClient); }
                    _Parent.NotifyClientConnected(Client.TcpClient);
                    Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
                }
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        private void _OnDataReceive(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;
            if (Client == null) return;

            try
            {
                byte[] ReceivedBytes = new byte[Client.TcpClient.Client.EndReceive(ar)];
                Array.Copy(Client.Buffer, ReceivedBytes, ReceivedBytes.Length);

                _Parent.NotifyDataReceived(ReceivedBytes, Client.TcpClient);

                Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
            }
            catch (SocketException) { lock (_ConnectedClients) { lock (_ConnectedClients) { _ConnectedClients.Remove(Client.TcpClient); _Parent.NotifyClientDisconnected(Client.TcpClient);  } } }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }
    }
}

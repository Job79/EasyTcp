using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;

namespace HenkTcp
{
    internal class ServerListener
    {
        private List<TcpClient> _ConnectedClients = new List<TcpClient>();
        public List<TcpClient> ConnectedClients { get { CheckClients(null, null); return _ConnectedClients.ToList(); } }
        public void Stop() { _Listener.Stop(); }

        private TcpListener _Listener;
        private Timer Timer = new Timer();
        private readonly int _BufferSize;
        private readonly int _MaxConnections;
        private readonly HenkTcpServer _Parent;

        public ServerListener(TcpListener Listener, HenkTcpServer Parent, int MaxConnections, int RemoveDisconnectedClientsTimeout, int BufferSize)
        {
            //start the listener
            _Listener = Listener;
            _Listener.Start();
            _Listener.BeginAcceptTcpClient(_OnClientConnect, _Listener);

            _Parent = Parent;
            _BufferSize = BufferSize;
            _MaxConnections = MaxConnections;

            //with the timer we will check for disconnected clients and remove them.
            Timer.Interval = RemoveDisconnectedClientsTimeout;
            Timer.Elapsed += CheckClients;
            Timer.AutoReset = false;
            Timer.Start();
        }

        private void _OnClientConnect(IAsyncResult ar)
        {
            try
            {
                if (_ConnectedClients.Count >= _MaxConnections) { _Listener.EndAcceptTcpClient(ar).Close(); _Listener.BeginAcceptTcpClient(_OnClientConnect, _Listener); }
                else
                {
                    ClientObject Client = new ClientObject();
                    Client.TcpClient = _Listener.EndAcceptTcpClient(ar);
                    Client.Buffer = new byte[_BufferSize];

                    _Listener.BeginAcceptTcpClient(_OnClientConnect, _Listener);

                    lock (_ConnectedClients) { _ConnectedClients.Add(Client.TcpClient); }
                    _Parent.NotifyClientConnected(Client.TcpClient);
                    Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
                }
            }
            catch { }
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
            catch { }
        }

        public void CheckClients(object source, ElapsedEventArgs e)
        {
            Parallel.ForEach(_ConnectedClients.ToList(), c =>
            {
                if (c.Client.Poll(0, SelectMode.SelectRead) && c.Available == 0)
                {
                    _Parent.NotifyClientDisconnected(c);
                    lock (_ConnectedClients) { _ConnectedClients.Remove(c); }
                }
            });
            Timer.Start();//enable the timer again
        }
    }
}

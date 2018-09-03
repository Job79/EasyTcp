using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Net;

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

                this.Listener = Listener;
                this.Listener.Start();
                this.Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        private void _OnClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient Client = Listener.EndAcceptTcpClient(ar);
                if (_ConnectedClients.Count >= _MaxConnections || _Parent.BannedIps.Contains(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString())) { Client.Close(); Console.WriteLine("Kicked client"); Listener.BeginAcceptTcpClient(_OnClientConnect, Listener); }
                else
                {
                    ClientObject ClientObject = new ClientObject() { TcpClient = Client, Buffer = new byte[_BufferSize] };

                    lock (_ConnectedClients) { _ConnectedClients.Add(Client); }
                    _Parent.NotifyClientConnected(Client);

                    Client.GetStream().BeginRead(ClientObject.Buffer, 0, ClientObject.Buffer.Length, _OnDataReceive, ClientObject);
                    Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);
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
                int ReceivedBytesCount = Client.TcpClient.Client.EndReceive(ar);
                if (ReceivedBytesCount <= 0)
                {
                    if (Client.TcpClient.Client.Poll(0, SelectMode.SelectRead)) lock (_ConnectedClients) { _ConnectedClients.Remove(Client.TcpClient); _Parent.NotifyClientDisconnected(Client.TcpClient); }
                    return;
                }

                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Array.Copy(Client.Buffer, ReceivedBytes, ReceivedBytes.Length);

                _Parent.NotifyDataReceived(ReceivedBytes, Client.TcpClient);
                Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
            }
            catch (SocketException) { lock (_ConnectedClients) { _ConnectedClients.Remove(Client.TcpClient); _Parent.NotifyClientDisconnected(Client.TcpClient); } }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }
    }
}

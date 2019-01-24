/* HenkTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace HenkTcp.Server
{
    internal class ServerListener
    {
        public HashSet<TcpClient> ConnectedClients { get; } = new HashSet<TcpClient>();

        public TcpListener Listener { get; }
        private readonly int _BufferSize;
        private readonly int _MaxConnections;
        private readonly int _MaxDataSize;
        private readonly HenkTcpServer _Parent;

        public ServerListener(TcpListener Listener, HenkTcpServer Parent, int MaxConnections, int BufferSize, int MaxDataSize)
        {
            try
            {
                _Parent = Parent;
                _BufferSize = BufferSize;
                _MaxConnections = MaxConnections;
                _MaxDataSize = MaxDataSize;

                this.Listener = Listener;
                this.Listener.Start();
                //Start accepting connections
                this.Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        private void _OnClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient Client = Listener.EndAcceptTcpClient(ar);//Accept client
                if (ConnectedClients.Count >= _MaxConnections)//Check if there are to many connections
                {
                    RefusedClient RefusedClient = new RefusedClient(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString(), true);
                    Client.Close();//Refuse connection
                    _Parent.NotifyOnRefusedConnection(RefusedClient);
                }
                else if (_Parent.BannedIPs.Contains(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString()))//Check if client is banned
                {
                    RefusedClient RefusedClient = new RefusedClient(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString(), false);
                    Client.Close();//Refuse connection
                    _Parent.NotifyOnRefusedConnection(RefusedClient);
                }
                else
                {
                    ClientObject ClientObject = new ClientObject() { TcpClient = Client, Buffer = new byte[_BufferSize] };

                    ConnectedClients.Add(Client);
                    _Parent.NotifyClientConnected(Client);

                    //Start listerning for data
                    Client.GetStream().BeginRead(ClientObject.Buffer, 0, ClientObject.Buffer.Length, _OnDataReceive, ClientObject);
                }
                Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);//Wait for next client
            }
            catch (Exception ex) { if (_Parent.IsRunning) _Parent.NotifyOnError(ex); }
        }

        private void _OnDataReceive(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;//Get ClientObject
            if (Client == null) return;

            try
            {
                //Test if client is connected
                if (Client.TcpClient.Client.Poll(0, SelectMode.SelectRead) && Client.TcpClient.Client.Available.Equals(0))
                {
                    lock (ConnectedClients) ConnectedClients.Remove(Client.TcpClient); //!Lock to acces list!
                    _Parent.NotifyClientDisconnected(Client.TcpClient); Client.TcpClient.Close(); return;
                }

                int ReceivedBytesCount = Client.TcpClient.Client.EndReceive(ar);
                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Buffer.BlockCopy(Client.Buffer, 0, ReceivedBytes, 0, ReceivedBytesCount);//Remove null bytes from buffer.

                if (Client.TcpClient.Available > 0 && Client.TcpClient.Available + Client.DataBuffer.Count <= _MaxDataSize)//When message is longer then the buffer can hold.
                {
                    Client.DataBuffer.AddRange(ReceivedBytes);
                    Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
                    return;
                }

                if (Client.DataBuffer.Count > 0)//When DataBuffer is used and no data is avaible next round
                {
                    Client.DataBuffer.AddRange(ReceivedBytes);
                    ReceivedBytes = Client.DataBuffer.ToArray();
                    Client.DataBuffer.Clear();
                }
              
                _Parent.NotifyDataReceived(ReceivedBytes, Client.TcpClient);
                Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
            }
            catch
            {
                lock (ConnectedClients) ConnectedClients.Remove(Client.TcpClient); //!Lock to acces list!
                _Parent.NotifyClientDisconnected(Client.TcpClient); Client.TcpClient.Close();
            }
        }
    }
}
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

namespace HenkTcp
{
    internal class ServerListener
    {
        public List<TcpClient> ConnectedClients { get; } = new List<TcpClient>();

        public TcpListener Listener { get; }
        private readonly int _BufferSize;
        private readonly int _MaxConnections;
        private readonly HenkTcpServer _Parent;
        private readonly bool _PrintDeniedMessage;

        public ServerListener(TcpListener Listener, HenkTcpServer Parent, int MaxConnections, int BufferSize, bool PrintDeniedMessage)
        {
            try
            {
                _Parent = Parent;
                _BufferSize = BufferSize;
                _MaxConnections = MaxConnections;
                _PrintDeniedMessage = PrintDeniedMessage;

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
                if (ConnectedClients.Count >= _MaxConnections || _Parent.BannedIps.Contains(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString())) {
                    if(_PrintDeniedMessage) Console.WriteLine($"[Server]Denied {((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString()}");
                    Client.Close(); Listener.BeginAcceptTcpClient(_OnClientConnect, Listener); }
                else
                {
                    ClientObject ClientObject = new ClientObject() { TcpClient = Client, Buffer = new byte[_BufferSize] };

                    ConnectedClients.Add(Client);
                    _Parent.NotifyClientConnected(Client);

                    Client.GetStream().BeginRead(ClientObject.Buffer, 0, ClientObject.Buffer.Length, _OnDataReceive, ClientObject);
                    Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);                   
                }
            }
            catch (Exception ex) { if (_Parent.IsRunning) _Parent.NotifyOnError(ex); }
        }

        private void _OnDataReceive(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;
            if (Client == null) return;

            try
            {
                int ReceivedBytesCount = Client.TcpClient.Client.EndReceive(ar);
                if (ReceivedBytesCount <= 0) { lock (ConnectedClients) ConnectedClients.Remove(Client.TcpClient);
                    _Parent.NotifyClientDisconnected(Client.TcpClient); return; }

                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Buffer.BlockCopy(Client.Buffer,0,ReceivedBytes,0,ReceivedBytesCount);

                Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
                _Parent.NotifyDataReceived(ReceivedBytes, Client.TcpClient);
            }
            catch { lock (ConnectedClients) ConnectedClients.Remove(Client.TcpClient);
                _Parent.NotifyClientDisconnected(Client.TcpClient); }
        }
    }
}

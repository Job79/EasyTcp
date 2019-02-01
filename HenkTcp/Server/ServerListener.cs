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
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HenkTcp.Server
{
    internal class ServerListener
    {
        /// <summary>
        /// Server listener.
        /// </summary>
        public TcpListener Listener { get; }

        /// <summary>
        /// HenkTcpServer class, will be used to call handlers.
        /// </summary>
        private readonly HenkTcpServer _Parent;

        /// <summary>
        /// HashSet of all current connected clients.
        /// </summary>
        public HashSet<TcpClient> ConnectedClients { get; } = new HashSet<TcpClient>();

        /// <summary>
        /// BufferSize, the size of the buffer at HenkTcp.Server/ClientObject/Buffer
        /// </summary>
        private readonly ushort _BufferSize;

        /// <summary>
        /// MaxConnections, max connected clients the server can have.
        /// </summary>
        private readonly int _MaxConnections;

        /// <summary>
        /// MaxDataSize, max bytes HenkTcp.Server/ClientObject/DataBuffer can have.
        /// </summary>
        private readonly int _MaxDataSize;


        public ServerListener(TcpListener Listener, HenkTcpServer Parent, int MaxConnections, ushort BufferSize, int MaxDataSize)
        {
            try
            {
                _Parent = Parent;
                _BufferSize = BufferSize;
                _MaxConnections = MaxConnections;
                _MaxDataSize = MaxDataSize;

                Listener.Server.DualMode = true;
                this.Listener = Listener;
                this.Listener.Start();

                //Start accepting connections
                this.Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Called when a new client connect's.
        /// </summary>
        private void _OnClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient Client = Listener.EndAcceptTcpClient(ar);//Accept connection
                if (_Parent.BannedIPs.Contains(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString()))//Check if client is banned
                {
                    RefusedClient RefusedClient = new RefusedClient(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString(), true);//IP, IsBanned
                    Client.Close();//Refuse connection
                    _Parent.NotifyClientRefused(RefusedClient);
                }
                else if (ConnectedClients.Count >= _MaxConnections)//Check if there are to many connections
                {
                    RefusedClient RefusedClient = new RefusedClient(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString(), false);//IP, IsBanned
                    Client.Close();//Refuse connection
                    _Parent.NotifyClientRefused(RefusedClient);
                }
                else
                {
                    ClientObject ClientObject = new ClientObject() { TcpClient = Client, Buffer = new byte[_BufferSize], DataBuffer = new List<byte>() };

                    ConnectedClients.Add(Client);
                    _Parent.NotifyClientConnected(Client);

                    //Start listerning for data
                    Client.GetStream().BeginRead(ClientObject.Buffer, 0, ClientObject.Buffer.Length, _OnDataReceive, ClientObject);
                }
            }
            catch (Exception ex) { if (_Parent.IsRunning) _Parent.NotifyOnError(ex); }

            Listener.BeginAcceptTcpClient(_OnClientConnect, Listener);//Wait for next client
        }

        /// <summary>
        /// Called when a client sended data to server.
        /// </summary>
        private void _OnDataReceive(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;//Get ClientObject

            try
            {
                //Test if client is connected.
                if (Client.TcpClient.Client.Poll(0, SelectMode.SelectRead) && Client.TcpClient.Client.Available.Equals(0)) { _CloseClientObject(Client); return; }

                int ReceivedBytesCount = Client.TcpClient.Client.EndReceive(ar);
                byte[] ReceivedBytes = new byte[ReceivedBytesCount];
                Buffer.BlockCopy(Client.Buffer, 0, ReceivedBytes, 0, ReceivedBytesCount);//Remove null bytes from buffer.

                //When message is longer then the buffer can hold and it is not bigger then MaxDataSize
                if (Client.TcpClient.Available > 0 && Client.TcpClient.Available + Client.DataBuffer.Count <= _MaxDataSize)
                {
                    Client.DataBuffer.AddRange(ReceivedBytes);
                    Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
                    return;
                }

                //When DataBuffer is used and no data is avaible next round OR DataBuffer is full.
                if (Client.DataBuffer.Any())
                {
                    Client.DataBuffer.AddRange(ReceivedBytes);
                    ReceivedBytes = Client.DataBuffer.ToArray();
                    Client.DataBuffer.Clear();
                }
                _Parent.NotifyDataReceived(ReceivedBytes, Client.TcpClient);
                Client.TcpClient.GetStream().BeginRead(Client.Buffer, 0, Client.Buffer.Length, _OnDataReceive, Client);
            }
            catch (SocketException) { _CloseClientObject(Client); }
            catch (Exception ex) { _CloseClientObject(Client); _Parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Close a connection.
        /// </summary>
        private void _CloseClientObject(ClientObject Client)
        {
            //~!Lock to acces list!~
            //~!This function is called by the async funtion _OnDataReceive!~
            lock (ConnectedClients) ConnectedClients.Remove(Client.TcpClient); 

            _Parent.NotifyClientDisconnected(Client.TcpClient);
            Client.TcpClient.GetStream().Close();
            Client.TcpClient.Close();
            Client.Buffer = null;
            Client.DataBuffer = null;
            //We cleared everything here,
            //Note that the GC will not Immediately remove everything from memory.
            //Some OverlappedData will stay in memory and wait very long for collecion,
            //please read: https://stackoverflow.com/questions/12296752/why-is-it-taking-so-long-to-gc-system-threading-overlappeddata
        }
    }
}
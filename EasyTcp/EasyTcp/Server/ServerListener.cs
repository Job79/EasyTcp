/* EasyTcp
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
using System.Net.Sockets;
using System.Collections.Generic;

namespace EasyTcp.Server
{
    internal class ServerListener
    {
        /// <summary>
        /// Listerning socket.
        /// </summary>
        public readonly Socket Listener;

        /// <summary>
        /// EasyTcpserver class, used to call handlers.
        /// </summary>
        private readonly EasyTcpServer _Parent;

        /// <summary>
        /// HashSet of all current connected clients.
        /// </summary>
        public HashSet<Socket> ConnectedClients { get; private set; } = new HashSet<Socket>();

        /// <summary>
        /// Max connected clients the server can have.
        /// </summary>
        private readonly int _MaxConnections;

        /// <summary>
        /// Max bytes the server can receive at 1 time.
        /// </summary>
        private readonly int _MaxDataSize;

        /// <summary>
        /// Set to false to stop the server.
        /// Else OnClienConnect will throw exeptions.
        /// </summary>
        public bool IsListerning = true;

        public ServerListener(Socket Listener, EasyTcpServer Parent, int MaxConnections, int MaxDataSize)
        {
            try
            {
                _Parent = Parent;
                _MaxConnections = MaxConnections;
                _MaxDataSize = MaxDataSize;

                this.Listener = Listener;
                this.Listener.Listen(100);//100 = maximum pending connections.

                //Start accepting new connections.
                this.Listener.BeginAccept(_OnClientConnect, Listener);
            }
            catch (Exception ex) { _Parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Called when a new client connect's.
        /// </summary>
        /// <param name="ar">Used to accept socket</param>
        private void _OnClientConnect(IAsyncResult ar)
        {
            if (!IsListerning) return;

            try
            {
                Socket Client = Listener.EndAccept(ar);//Accept socket.

                if (_Parent.BannedIPs.Contains(((IPEndPoint)Client.RemoteEndPoint).Address.ToString()))//Check if client is banned.
                    _RefuseClient(Client, true);///Refuse connection and call <see cref="HenkTcpServer.ClientRefused"/>.
                else if (ConnectedClients.Count >= _MaxConnections)//Check if there are to many connections.
                    _RefuseClient(Client, false);///Refuse connection and call <see cref="HenkTcpServer.ClientRefused"/>.
                else
                {
                    ClientObject ClientObject = new ClientObject() { Socket = Client, Buffer = new byte[4] };

                    ConnectedClients.Add(Client);
                    _Parent.NotifyClientConnected(Client);

                    //Start listerning for data.
                    Client.BeginReceive(ClientObject.Buffer, 0, ClientObject.Buffer.Length, SocketFlags.None, _ReceiveLength, ClientObject);
                }
            }
            catch (Exception ex) { if (_Parent.IsRunning) _Parent.NotifyOnError(ex); }

            Listener.BeginAccept(_OnClientConnect, Listener);//Wait for next client
        }

        /// <summary>
        /// Refuse a connection, Called by _OnClientConnect.
        /// </summary>
        private void _RefuseClient(Socket Client, bool IsBanned)
        {
            string IP = ((IPEndPoint)Client.RemoteEndPoint).Address.ToString();
            Client.Close();
            _Parent.NotifyClientRefused(new RefusedClient(IP, IsBanned));
        }

        /// <summary>
        /// Receive length of a message, triggert first when receiving a message.
        /// </summary>
        /// <param name="ar"></param>
        private void _ReceiveLength(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;

            try
            {
                //Test if client is connected.
                if (Client.Socket.Poll(0, SelectMode.SelectRead) && Client.Socket.Available.Equals(0)) { _CloseClientObject(Client); return; }

                int DataLength = BitConverter.ToInt32(Client.Buffer, 0);//Get the length of the data.

                if (DataLength <= 0 || DataLength > _MaxDataSize) _CloseClientObject(Client);//Invalid length, close connection.
                else Client.Socket.BeginReceive(Client.Buffer = new byte[DataLength], 0, DataLength, SocketFlags.None, _ReceiveData, Client);//Start accepting the data.
            }
            catch (SocketException) { _CloseClientObject(Client); }
            catch (Exception ex) { _CloseClientObject(Client); _Parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Receive data, triggerd after <see cref="_ReceiveLength(IAsyncResult)"/>
        /// </summary>
        /// <param name="ar">Contains <see cref="ClientObject"/></param>
        private void _ReceiveData(IAsyncResult ar)
        {
            ClientObject Client = ar.AsyncState as ClientObject;

            try
            {
                //Test if client is connected.
                if (Client.Socket.Poll(0, SelectMode.SelectRead) && Client.Socket.Available.Equals(0)) { _CloseClientObject(Client); return; }

                _Parent.NotifyDataReceived(Client.Buffer, Client.Socket);//Trigger event
                Client.Socket.BeginReceive(Client.Buffer = new byte[4], 0, Client.Buffer.Length, SocketFlags.None, _ReceiveLength, Client);//Start receiving next length.
            }
            catch (SocketException) { _CloseClientObject(Client); }
            catch (Exception ex) { _CloseClientObject(Client); _Parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Close a disconnected connetion, Called by _OnDataReceive.
        /// </summary>
        private void _CloseClientObject(ClientObject Client)
        {
            ///~!This function is called by an async funtion(<see cref="_OnDataReceive(IAsyncResult)"/>) so need to be locked!~
            lock (ConnectedClients) ConnectedClients.Remove(Client.Socket);

            ///Call <see cref="HenkTcpServer.ClientDisconnected"/> 
            _Parent.NotifyClientDisconnected(Client.Socket);

            Client.Socket.Shutdown(SocketShutdown.Both);
            Client.Socket.Close();
            Client.Buffer = null;
        }
    }
}

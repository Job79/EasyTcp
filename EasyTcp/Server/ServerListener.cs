/* EasyTcp
 * 
 * Copyright (c) 2019 henkje
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
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
        private readonly EasyTcpServer parent;

        /// <summary>
        /// HashSet of all current connected clients.
        /// </summary>
        public HashSet<Socket> ConnectedClients { get; private set; } = new HashSet<Socket>();

        /// <summary>
        /// Max connected clients the server can have.
        /// </summary>
        private readonly int maxConnections;

        /// <summary>
        /// Max bytes the server can receive at 1 time.
        /// </summary>
        private readonly ushort maxDataSize;

        /* Set to false to stop the server.
         * Else OnClienConnect will throw exeptions.*/
        public bool IsListerning = true;

        public ServerListener(Socket listener, EasyTcpServer parent, int maxConnections, ushort maxDataSize)
        {
            try
            {
                this.parent = parent;
                this.maxConnections = maxConnections;
                this.maxDataSize = maxDataSize;

                Listener = listener;
                Listener.Listen(100);//100 = maximum pending connections.

                //Start accepting new connections.
                Listener.BeginAccept(OnClientConnect, null);
            }
            catch (Exception ex) { parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Called when a new client connect's.
        /// </summary>
        /// <param name="ar">Used to call EndAccept</param>
        private void OnClientConnect(IAsyncResult ar)
        {
            if (!IsListerning) return;

            try
            {
                Socket client = Listener.EndAccept(ar);//Accept socket.

                if (parent.BannedIPs.Contains(((IPEndPoint)client.RemoteEndPoint).Address.ToString()))//Check if client is banned.
                    RefuseClient(client, true);///Refuse connection and call <see cref="HenkTcpServer.ClientRefused"/>.
                else if (ConnectedClients.Count >= maxConnections)//Check if there are to many connections.
                    RefuseClient(client, false);///Refuse connection and call <see cref="HenkTcpServer.ClientRefused"/>.
                else
                {
                    ClientObject clientObject = new ClientObject() { Socket = client, Buffer = new byte[2] };

                    ConnectedClients.Add(client);
                    parent.NotifyClientConnected(client);

                    //Start listerning for data.
                    client.BeginReceive(clientObject.Buffer, 0, clientObject.Buffer.Length, SocketFlags.None, OnReceiveLength, clientObject);
                }
            }
            catch (Exception ex) { if (parent.IsRunning) parent.NotifyOnError(ex); }

            Listener.BeginAccept(OnClientConnect, Listener);//Wait for next client
        }

        /// <summary>
        /// Refuse a connection,Called by onClientConnect.
        /// </summary>
        private void RefuseClient(Socket client, bool isBanned)
        {
            string IP = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
            client.Close();
            parent.NotifyClientRefused(new RefusedClient(IP, isBanned));
        }

        /// <summary>
        /// Receive length of a message, triggert first when receiving a message.
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiveLength(IAsyncResult ar)
        {
            ClientObject client = ar.AsyncState as ClientObject;

            try
            {
                //Test if client is connected.
                if (client.Socket.Poll(0, SelectMode.SelectRead) && client.Socket.Available.Equals(0))
                { CloseClientObject(client); return; }

                ushort dataLength = BitConverter.ToUInt16(client.Buffer, 0);//Get the length of the data.

                if (dataLength <= 0 || dataLength > maxDataSize) CloseClientObject(client);//Invalid length, close connection.
                else client.Socket.BeginReceive(client.Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None, OnReceiveData, client);//Start accepting the data.
            }
            catch (Exception ex) { CloseClientObject(client); parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Receive data, triggerd after <see cref="ReceiveLength(IAsyncResult)"/>
        /// </summary>
        /// <param name="ar">Contains <see cref="ClientObject"/></param>
        private void OnReceiveData(IAsyncResult ar)
        {
            ClientObject client = ar.AsyncState as ClientObject;

            try
            {
                //Test if client is connected.
                if (client.Socket.Poll(0, SelectMode.SelectRead) && client.Socket.Available.Equals(0))
                { CloseClientObject(client); return; }

                parent.NotifyDataReceived(client.Buffer, client.Socket);//Trigger event
                client.Socket.BeginReceive(client.Buffer = new byte[2], 0, client.Buffer.Length, SocketFlags.None, OnReceiveLength, client);//Start receiving next length.
            }
            catch (Exception ex) { CloseClientObject(client); parent.NotifyOnError(ex); }
        }

        /// <summary>
        /// Close a disconnected connetion, Called by OnDataReceive.
        /// </summary>
        private void CloseClientObject(ClientObject Client)
        {
            ///~!This function is called by an async funtion(<see cref="_OnDataReceive(IAsyncResult)"/>) so need to be locked!~
            lock (ConnectedClients) ConnectedClients.Remove(Client.Socket);

            ///Call <see cref="HenkTcpServer.ClientDisconnected"/> 
            parent.NotifyClientDisconnected(Client.Socket);

            Client.Socket.Shutdown(SocketShutdown.Both);
            Client.Socket.Close();
            Client.Buffer = null;
        }
    }
}

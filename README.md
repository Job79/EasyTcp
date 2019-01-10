
# HenkTcp
HenkTcp is a async, fast, simple tcp server/client library.
HenkTcp is easy to use and can be used with strings, and byte arrays.
It supports encryption, the encryption can be used with AES and the other symmetric algorithms(DES/TripleDES).

# How do i use HenkTcp?
1. Add the nuget package to your application or download this project and import the classes.
2. To create a server you can use this example:
```cs
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using HenkTcp;

namespace HenkTcpServerExample
{
    class Program
    {
        static HenkTcpServer Server = new HenkTcpServer();

        static void Main(string[] args)
        {
            /* All overloads:
             * Without encryption
            Start(int Port, int MaxConnections = 10000, int BufferSize = 1024, bool PrintDeniedMessage = true)
            Start(string Ip, int Port, int MaxConnections, int BufferSize = 1024, bool PrintDeniedMessage = true)
            Start(IPAddress Ip, int Port, int MaxConnections, int BufferSize = 1024, bool PrintDeniedMessage = true)

            * With encryption
            * If keysize is equal to 0 HenkTcp will automatically get the right KeySize.
            Start(string Ip, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024, bool PrintDeniedMessage = true)
            Start(IPAddress Ip, int Port, int MaxConnections, string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0, int BufferSize = 1024, bool PrintDeniedMessage = true)
            Start(string Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024, bool PrintDeniedMessage = true)
            Start(IPAddress Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024, bool PrintDeniedMessage = true)
            */

            //Examples:
            //Start server on 0.0.0.0/52525 with max 10000 connections and deniedmessages will be printed in the console.
            ///Server.Start("0.0.0.0",52525,10000, true);

            //Start server with AES256 encryption and a Password/Salt.
            Server.Start("0.0.0.0", 52525, 10000, "password", "YourSalt");

            //Advanced way to start a server with encryption:
            //Create a key of 256 bits(32 bytes) with 100000 interation(using PBKDF2) and with Password/Salt.
            ///byte[] Key = Encryption.CreateKey(Aes.Create(),"password","YourSalt",100000,32);
            //Now we start the server with our own key and AES encryption.
            ///Server.Start("0.0.0.0", 52525, 10000, Aes.Create(),Key);

            //Start server on IPAddress.Any/1234.
            //Server.Start(1234);
            //Server can also be started with the System.Net.IPAddress

            /* Set encryption
             * Encryption can also be changed/enabled with SetEncryption.
                SetEncryption(string Password, string Salt = "HenkTcpSalt", int Iterations = 10000, int KeySize = 0)
                SetEncryption(SymmetricAlgorithm Algorithm, byte[] EncryptionKey)
             */

            /* Event Hanlers,
             * 
            EventHandler<TcpClient> ClientConnected
             * Will be called if a new client connected,
             * WILL NOT BE CALLED IF CLIENT IS DENIED.
             * Client will be denied if his ip is in the BannedClients list OR if the server reach max connected clients.
             * 
            EventHandler<TcpClient> ClientDisconnected
             * Will be called if client Disconnects
             * 
            EventHandler<Message> DataReceived
             * Will be called if server received data
             * 
            EventHandler<Exception> OnError
             * Will be called if server gets any error.
             */

            //Set EventHandlers:
            Server.DataReceived += DataReceived;
            Server.ClientConnected += ClientConnected;
            Server.ClientDisconnected += ClientDisconnect;
            //EventHandlers can also be set like this:
            //NOTE, if a event is set like this it can't be removed, but here it isn't a problem.
            Server.OnError += (object sender, Exception e) => { Console.WriteLine(e.ToString()); };

            /* Ban Ips,
             * Ips can be banned, Client will be denied if he connects.
             * !SERVER WILL NOT KICK THE CLIENT!
             * No event will be launched, Console will print the following line if enabled on startup:
             * "[Server]Denied {IP}"
             */

            //We can ban ips like this:
            Server.BannedIps.Add("123.123.123.123");
            //And unban them:
            Server.BannedIps.Remove("123.123.123.123");
            //Or clear all banned Ips.
            Server.BannedIps.Clear();
            //We can also copy a whole list to it.
            ///Server.BannedIps = BannedIps;

            /* ConnectedClients & ConnectedClientsCount
             * Get the list of connectedclients or only the count.
                List<TcpClient> ConnectedClients
                int ConnectedClientsCount
             */
            //Example:
            ///int ConnectedClientsCount = Server.ConnectedClientsCount;
            ///List <TcpClient> ConnectedClients = Server.ConnectedClients;

            /* Listener
             * Get the TcpListener of the server.
             TcpListener Listener
             */
            ///TcpListener Listener = Server.Listener;

            /* IsRunning
             * Get the state of the server.
              bool IsRunning
             */
            Console.WriteLine(Server.IsRunning ? "The server is running" : "The server is offline");

            /* Stop
             * Stop the server.
             * Will execute TcpListener.Stop();
              Stop()
             */
            ///Server.Stop();
            ///

            /* Broadcast
             * Broadcast will send a message to all connected clients
            Broadcast(string Data)              UTF8 will be used for strings
            Broadcast(byte[] Data)

             * BroadcastEncrypted
             * BroadcastEncrypted will send a message to all clients.
             * The message will be encrypted with our selected algorithm/key.
            BroadcastEncrypted(string Data)     UTF8 will be used for strings
            BroadcastEncrypted(byte[] Data)
             */
            //Example:
            ///Server.Broadcast("Hello World!");
            ///Server.BroadcastEncrypted("Hello World!");

            /* Send
             * Send a message to 1 client.
            Send(TcpClient Client, string Data)             UTF8 will be used for strings
            Send(TcpClient Client, byte[] Data)

             * SendEncrypted
             * Send a message to 1 client.
             * The message will be encrypted with our selected algorithm/key.
            SendEncrypted(TcpClient Client, string Data)    UTF8 will be used for strings
            SendEncrypted(TcpClient Client, byte[] Data)
             */

            /* SendAndGetReply
             * Send a message to a client and wait for a reply.
            Message SendAndGetReply(TcpClient Client, string Text, TimeSpan Timeout)            UTF8 will be used for strings
            Message SendAndGetReply(TcpClient Client, byte[] Data, TimeSpan Timeout)
             * SendAndGetReply
             * Send a message to a client and wait for a reply.
             * The message will be encrypted with our selected algorithm/key.
            Message SendAndGetReplyEncrypted(TcpClient Client, string Text, TimeSpan Timeout)   UTF8 will be used for strings
            Message SendAndGetReplyEncrypted(TcpClient Client, byte[] Data, TimeSpan Timeout)
             */
            //Example:
            ///Message e = Server.SendAndGetReply(Client,"HelloWorld!",TimeSpan.FromSeconds(5));

            //!~Infinite wait~!
            Task.Delay(-1);
        }

        private static void DataReceived(object sender, Message e)
        {
            /* TcpClient
             * Get the TcpClient who send the message.
            readonly TcpClient TcpClient;
             */

            /* Data
             * Get the Received Data.
            readonly byte[] Data;
             */

            /* DecryptedData
             * Get the Received Data decrypted.
            byte[] DecryptedData;
             * IF ALGORITHM/KEY IS INCORRECT DECRYPTEDDATA WILL RETURN NULL.
             */

            /* MessageString
             * Get the received string.
             string MessageString;              UTF8 will be used for strings
             */

            /* DecryptedMessageString
             * Get the received string decrypted.
             string DecryptedMessageString;     UTF8 will be used for strings
             * IF ALGORITHM/KEY IS INCORRECT DECRYPTEDMESSAGESTRING WILL THROW AN ERROR.
             */

            /* ClientIP
             * Get the Ip of the TcpClient who send the message.
             */

            /* Reply
             * Send data back to Client.
            Reply(string data)                  UTF8 will be used for strings
            Reply(byte[] data)

             * ReplyEncrypted
             * Send data back to Client encrypted.
            ReplyEncrypted(string Data)         UTF8 will be used for strings
            ReplyEncrypted(byte[] Data)
             */

            Console.WriteLine($"Received: {e.MessageString}");
        }

        private static void ClientConnected(object sender, TcpClient e)
        {
            Console.WriteLine($"Client {e.GetHashCode()} connected");
        }

        private static void ClientDisconnect(object sender, TcpClient e)
        {
            Console.WriteLine($"Client {e.GetHashCode()} Disconnect");
        }
    }
}

```

3. T create a client you can use the following:

```cs
using System;
using HenkTcp;
using System.Security.Cryptography;

namespace HenkTcpClientExample
{
    class Program
    {
        static HenkTcpClient Client = new HenkTcpClient();

        static void Main(string[] args)
        {
            //first we will set the handlers of the client
            Client.DataReceived += DataReceived;
            Client.OnDisconnect += Disconnected;
            //handlers can also be set like this:
            Client.OnError += (object sender, Exception e) => { Console.WriteLine(e.ToString()); };

            //connect to 127.0.0.1:52525 witch can take max 1 second with a password and salt for the encryption
            if (Client.Connect("127.0.0.1", 52525, TimeSpan.FromSeconds(1), "password", "YourSalt"))
            {
                //connect with key advanced way
                //create a key of 256 bits(32 bytes) and with 100000 interation(using PBKDF2) with the salt "salt"
                //byte[] Key = Encryption.CreateKey(Aes.Create(),"password","YourSalt",100000,32);
                //Client.Connect("127.0.0.1", 52525, TimeSpan.FromSeconds(1), Aes.Create(),Key);

                //connect without encryption used:
                //Client.Connect("127.0.0.1", 52525, TimeSpan.FromSeconds(1));

                while (Console.ReadKey().Key != ConsoleKey.C)
                {
                    Console.WriteLine(Client.IsConnected ? "The client is connected" : "the client is disconnected");

                    Console.WriteLine("enter a message to send to the server:");
                    Client.Write(Console.ReadLine());

                    //encrypted way:
                    //the client need to be connected with encryption enabled for this
                    //Console.WriteLine("enter a message to send to the server encrypted:");
                    //Client.WriteEncrypted(Console.ReadLine());

                    //write and wait for a reply,
                    //timespan= time that it can take before resume, if not received it will return null

                    //Message Reply = Client.WriteAndGetReply("hey",TimeSpan.FromSeconds(5));
                    //if (Reply == null) return;
                    //now it can be used like in the datareceived
                    //string s = Reply.MessageString;

                    //encrypted way
                    //Message Reply = Client.WriteAndGetReplyEncrypted("hey", TimeSpan.FromSeconds(5));
                }
                //disconnect from server, without triggering ondisconnect
                Client.Disconnect();
                //disconnect from server with triggering ondisconnect
                Client.Disconnect(true);
            }
            else { Console.WriteLine("Could not connect with the server :("); Console.ReadLine(); }
        }

        private static void Disconnected(object sender, HenkTcpClient e)
        {
            Console.WriteLine("Disconnected from server");
        }

        private static void DataReceived(object sender, Message e)
        {
            Console.WriteLine($"received: {e.MessageString}");

            //display encrypted data:
            //Console.WriteLine($"received: {e.DecryptedMessageString}");

            //byte[] data = e.Data;//get bytes of message
            //byte[] decrypted = e.DecryptedData;
            //e.Reply("reply!");//reply message
            //e.ReplyEncrypted("reply encrypted!");
        }
    }
}
```
4. You're finished

# contributing
Contact me at discord(henkje#0033) or send my a email(henkje@pm.me).

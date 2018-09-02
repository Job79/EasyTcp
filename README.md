# how do i use HenkTcp?

1. add the nuget to your application.
2. to create a server you can use this example:
```cs
using System;
using System.Net.Sockets;
using HenkTcp;
using System.Security.Cryptography;

namespace async_tcp_server
{
    class Program
    {
        static HenkTcpServer server = new HenkTcpServer();

        static void Main(string[] args)
        {
            //start the server on port 52525 on ip 0.0.0.0 and with max 10000 connections
            server.Start("0.0.0.0", 52525, 10000);

            //start the server, but with encryption enabled
            //need System.Security.Cryptography for this.
            //  server.Start("0.0.0.0", 52525, 10000, Aes.Create(), Encryption.CreateKey(Aes.Create(), "Password", Salt: "YourSalt"));

            //set the handlers
            server.ClientConnected += ClientConnected;
            server.DataReceived += DataReceived;
            server.ClientDisconnected += ClientDisconnect;
            server.OnError += (object sender, Exception e) => { Console.WriteLine(e.ToString()); };

            while (true)
            {
                Console.ReadLine();
                //get the count off all conected clients
                Console.WriteLine(server.ConnectedClientsCount);
                //send a message to all clients
                server.Broadcast("Hey from server!");

                //send a broadcast encrypted
                //server.BroadcastEncrypted("Hey from server!");
            }
        }

        private static void ClientDisconnect(object sender, TcpClient e)
        {
            Console.WriteLine($"Client Disconnect {e.GetHashCode()}");
        }

        private static void DataReceived(object sender, Message e)
        {
            Console.WriteLine($"received: {e.MessageString} from: {e.TcpClient.GetHashCode()} ip:{e.senderIP}");
            //reply the client
            e.Reply(e.Data);

            //reply the data encrypted
            //e.ReplyEncrypted(e.Data);
        }

        private static void ClientConnected(object sender, TcpClient e)
        {
            Console.WriteLine($"Client connected {e.GetHashCode()}");
            //send a message to a tcpclient
            server.Write(e,"Welcome to the server");

            //send message encrypted
            //server.WriteEncrypted(e, "Welcome to the server");
        }
    }
}
```

4. to create a client you can use the following:

```cs
using System;
using HenkTcp;
using System.Security.Cryptography;

namespace async_TcpClient
{
    class Program
    {
        private static HenkTcpClient _Client = new HenkTcpClient();

        static void Main(string[] args)
        {
            //enable the handlers
            _Client.DataReceived += DataReceived;
            _Client.OnDisconnect += Disconnected;
            _Client.OnError += (object sender, Exception e) => { Console.WriteLine(e.ToString()); };
                
            //connect with 127.0.0.1 on port 52525 witch can take maximum 1 second.
            if (_Client.Connect("127.0.0.1", 52525, TimeSpan.FromSeconds(1)))
            //connect but with encryption enabled.
            //need System.Security.Cryptography for this.
            //if (_Client.Connect("192.168.1.11",52525,TimeSpan.FromSeconds(1), Encryption.CreateKey(Aes.Create(), "Password", Salt: "YourSalt")))
            {
                while (true)
                {
                    //write data to server
                    _Client.Write(Console.ReadLine());

                    //get reply from the server
                    //_Client.DataReceived -= DataReceived;//disable the datareceiver so it will not be triggerd
                    //string reply = _Client.WriteAndGetReply("get reply",TimeSpan.FromSeconds(5)).MessageString;
                    //Console.WriteLine($"reply:{reply}");
                    //_Client.DataReceived += DataReceived;//enable again
                }
            }
        }

        private static void Disconnected(object sender, HenkTcpClient e)
        {
            Console.WriteLine("Disconnected");

            if (e.Connect("127.0.0.1", 52525, TimeSpan.FromSeconds(1)))
            {
                Console.WriteLine("reconnected");
                //resume connected
            }
            else
            {
                //close application
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void DataReceived(object sender, Message e)
        {
            //show a normal message
            Console.WriteLine($"Received: {e.MessageString}");

            //show a encrypted message
            //Console.WriteLine($"ReceivedEncrypted: {e.DecryptedMessageString}");
        }
    }
}
```
4. you are done.

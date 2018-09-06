
# what is HenkTcp?

HenkTcp is a async, fast, simple tcp server/client library.
HenkTcp is easy to use and can be used with strings, and byte arrays.
It also support encryption, the encryption can be used with AES and the other symmetric algorithms(DES/TrippleDES).

# how do i use HenkTcp?

1. add the nuget package to your application.
2. to create a server you can use this example:
```cs
using System;
using System.Net.Sockets;
using HenkTcp;
using System.Security.Cryptography;

namespace HenkTcpServerExample
{
    class Program
    {
        static HenkTcpServer Server = new HenkTcpServer();

        static void Main(string[] args)
        {
            //start server on 0.0.0.0 on port 52525 with max 10000 connections
            //Server.Start("0.0.0.0",52525,10000);
            //start server with aes encryption enabled and a password and salt
            Server.Start("0.0.0.0", 52525, 10000, "password","YourSalt");
            //advanced way to start a server with encryption
            //create a key of 256 bits(32 bytes) and with 100000 interation(using PBKDF2) with the salt "salt"
            //byte[] Key = Encryption.CreateKey(Aes.Create(),"password","YourSalt",100000,32);
            //Server.Start("0.0.0.0", 52525, 10000, Aes.Create(),Key);

            //start server on port 1234 on IPAddress.Any
            //this way can't be used with encryption
            //Server.Start(1234);
            //server can also be started with the System.Net.IPAddress

            //now we will set the event handlers
            Server.DataReceived += DataReceived;
            Server.ClientConnected += ClientConnected;
            Server.ClientDisconnected += ClientDisconnect;
            //handlers can also be set like this:
            Server.OnError += (object sender, Exception e) => { Console.WriteLine(e.ToString()); };

            //we can ban ips like this:
            Server.BannedIps.Add("123.123.123.123");
            //and unban them:
            Server.BannedIps.Remove("123.123.123.123");
            //or clear them:
            Server.BannedIps.Clear();


            while (Console.ReadKey().Key != ConsoleKey.C)
            {
                Console.WriteLine(Server.IsRunning ? "The server is running" : "The server is offline");
                Console.WriteLine($"there are {Server.ConnectedClientsCount} clients connected");

                Console.WriteLine("enter a message to send to all users:");
                Server.Broadcast(Console.ReadLine());

                //encrypted way(use only if server is started with encryption enabled)
                /*
                Console.WriteLine("enter a message to send to all users:");
                Server.BroadcastEncrypted(Console.ReadLine());
                */
            }
            //stop the server
            Server.Stop();
        }

        private static void ClientDisconnect(object sender, TcpClient e)
        {
            Console.WriteLine($"Client {e.GetHashCode()} Disconnect");
        }

        private static void DataReceived(object sender, Message e)
        {
            Console.WriteLine($"received: {e.MessageString} from: {e.TcpClient.GetHashCode()} ip:{e.SenderIP}");

            //display encrypted data:
            //Console.WriteLine($"received: {e.DecryptedMessageString} from: {e.TcpClient.GetHashCode()} ip:{e.SenderIP}");
            //byte[] data = e.Data;//get bytes of message
            //byte[] decrypted = e.DecryptedData;
            e.Reply("reply!");//reply message
            //e.ReplyEncrypted("reply encrypted!");

            //get tcpclient
            //TcpClient Client = e.TcpClient;
        }

        private static void ClientConnected(object sender, TcpClient e)
        {
            Console.WriteLine($"Client {e.GetHashCode()} connected");

            //write something
            Server.Write(e,"hey from server");
            //write encrypted
            //Server.WriteEncrypted(e,"hey encrypted!");

            //write and wait for a reply,
            //e = tcpclient
            //timespan= time that it can take before resume, if not received it will return null

            //Message Reply = Server.WriteAndGetReply(e,"hey",TimeSpan.FromSeconds(5));
            //if (Reply == null) return;
            //not it can be used like in the datareceived
            //string s = Reply.MessageString;

            //encrypted way
            //Message Reply = Server.WriteAndGetReplyEncrypted(e, "hey", TimeSpan.FromSeconds(5));
        }
    }
}
```

3. to create a client you can use the following:

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
                    //e = tcpclient
                    //timespan= time that it can take before resume, if not received it will return null

                    //Message Reply = Client.WriteAndGetReply("hey",TimeSpan.FromSeconds(5));
                    //if (Reply == null) return;
                    //not it can be used like in the datareceived
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
4. you are done.

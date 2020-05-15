<p align="center">
  <b>EasyTcp</b>
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img src="https://img.shields.io/badge/version-3.0.0.0-green.svg">
  <img src="https://img.shields.io/badge/build-passing-green.svg">
  <br/>
  Easy and simple library for TCP clients and servers. Focused on performance and usability.
  <br/><br/><br/>
</p>

# EasyTcp
Packege with the basic functions of EasyTcp.<br/>
Contains the EasyTcpServer and EasyTcpClient. These classes are a simple wrapper around the Socket class. <br>
Adding event handlers and functions to make sending and processing received data simple. <br/>
Check out these examples, and see the EasyTcp.Examples folder for more.
```cs
using var server = new EasyTcpServer().Start(PORT);
server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
server.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message.ToString()}");
```

```cs
using var client = new EasyTcpClient();
client.OnConnect += (sender, client) => Console.WriteLine("Client connected!");
client.OnDisconnect += (sender, client) => Console.WriteLine("Client disconnected!");
client.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message.ToString()}");
            
if(!client.Connect("127.0.0.1", PORT)) return; 
client.Send("Hello server");
```

# EasyTcp.Actions
Package that adds support for 'actions' <br/>
Actions are functions with a specific attribute that get triggered when a message is received. This happens based on the received data and is perfect for medium/big servers or clients.<br/>
Check these examples out, and see the EasyTcp.Examples folder for more.
```cs
public static void Run()
{
    using var server = new EasyTcpActionServer().Start(PORT);
    server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
    server.OnDisconnect += (sender, client) =>
        Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
}

[EasyTcpAction("ECHO")]
public static void EchoAction(object sender, Message message)
{
    message.Client.Send(message);
}

[EasyTcpAction("BROADCAST")]
public static void BroadCastAction(object sender, Message message)
{
    var server = (EasyTcpServer) sender;
    server.SendAll(message);
}
```

```cs
using var client = new EasyTcpClient();
if(!client.Connect(IPAddress.Loopback, PORT)) return; 
client.SendAction("ECHO","Hello server"); // Trigger the ECHO action server side
client.SendAction("BROADCAST","Hello everyone"); // Trigger the BROADCAST action server side
```

# EasyTcp.Encryption
Package containing support for encrypting EasyTcpPackages. <br/>
This includes the Message class and custom EasyTcpPackeges. <br/>
Here are some more examples:
```cs
using var encrypter = new EasyEncrypt("Password","SALT1415312");
using var server = new EasyTcpServer().Start(PORT);
server.OnDataReceive += (sender, message) => 
    Console.WriteLine($"Received: {message.Decrypt(encrypter).ToString()}");

```
```cs
using var encrypter = new EasyEncrypt("Password","SALT1415312");
using var client = new EasyTcpClient();
if (!client.Connect(IPAddress.Loopback, PORT)) return;
client.Send(EasyTcpPacket.To<Message>("Hello Server!").Encrypt(encrypter));
```

# Contribution / Help / Questions
Create a issue, pull request or send an email to jobse@pm.me


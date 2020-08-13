<p align="center">
  <img src="icon.png" width="300px">
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img alt="Nuget" src="https://img.shields.io/nuget/v/EasyTcp">
  <img alt="Nuget" src="https://img.shields.io/nuget/dt/EasyTcp">
  <a href="https://matrix.to/#/!UfWuzAAgKkyPzNyPxM:matrix.org?via=matrix.org"><img alt="Matrix" src="https://img.shields.io/matrix/EasyTcp3:matrix.org"></a>
  <img alt="GitHub stars" src="https://img.shields.io/github/stars/job79/EasyTcp">
  <br/>
  Easy and simple library for TCP clients and servers. Focused on performance and usability.
  <br/><br/><br/>
</p>

# Why EasyTcp
- Very simple
- Lightweight & High performance (~430.000 round trips/second)*
- EasyTcp.Actions wich makes creating API's realy simple
- Support for sending raw streams/very large arrays
- Serialistaion and compression
- Different kinds of framing
- Async support
- Unit tested (~250 unit tests)

\* Tested on local machine(linux, ryzen 7) with clients and server running under the same process. See EasyTcp.Examples/SpeedTest/ThroughputTest.cs

# Packages
## EasyTcp
EasyTcp3 is a package that makes creating tcp servers and clients simple. <br/>
It is very fast, simple, supports framing, serialisation, disconnect detection, event handlers and more. <br/>
See the [EasyTcp.Examples](https://github.com/Job79/EasyTcp/tree/master/EasyTcp3/EasyTcp3.Examples) folder for documentation.
```cs
using var server = new EasyTcpServer().Start(PORT);
server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
server.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message}");
```

```cs
using var client = new EasyTcpClient();
client.OnConnect += (sender, client) => Console.WriteLine("Client connected!");
client.OnDisconnect += (sender, client) => Console.WriteLine("Client disconnected!");
client.OnDataReceive += (sender, message) => Console.WriteLine($"Received: {message}");
            
if(!client.Connect("127.0.0.1", PORT)) return; 
client.Send("Hello server");
```

## EasyTcp.Actions
EasyTcp.Actions adds support to EasyTcp for triggering functions based on received data. <br/>
It does this without giving up (noticeable) performance, and makes creating big servers/clients easy. <br/>
See the [EasyTcp.Examples](https://github.com/Job79/EasyTcp/tree/master/EasyTcp3/EasyTcp3.Examples) folder for documentation.
```cs
public static void Run()
{
    using var server = new EasyTcpActionServer().Start(PORT);
    server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
    server.OnDisconnect += (sender, client) =>
        Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
}

[EasyTcpAction("ECHO")]
public void EchoAction(Message message)
{
    message.Client.Send(message);
}

[EasyTcpAction("BROADCAST")]
public void BroadCastAction(object sender, Message message)
{
    var server = (EasyTcpServer) sender;
    server.SendAll(message);
}
```

```cs
using var client = new EasyTcpClient();
if(!client.Connect("127.0.0.1", PORT)) return; 
client.SendAction("ECHO","Hello me"); // Trigger the ECHO action server side
client.SendAction("BROADCAST","Hello everyone"); // Trigger the BROADCAST action server side
```

## EasyTcp.Encryption
EasyTcp.Encryption adds ssl and custom encryption support to EasyTcp. <br/>
See the [EasyTcp.Examples](https://github.com/Job79/EasyTcp/tree/master/EasyTcp3/EasyTcp3.Examples) folder for documentation.
```cs
using var certificate = new X509Certificate2("certificate.pfx", "password");
using var server = new EasyTcpServer().UseSsl(certificate).Start(PORT);

server.OnDataReceive += (sender, message) => Console.WriteLine(message); // Message is automatically decrypted
```
```cs
using var client = new EasyTcpClient().UseSsl("localhost",true); 
if(!client.Connect("127.0.0.1", PORT)) return;
client.Send("Hello ssl server!"); // All data is automatically encrypted
```
<br/><br/>
```cs
using var encrypter = new EasyEncrypt("Password", "Salt531351235");
using var server = new EasyTcpServer().UseEncryption(encrypter).Start(PORT);

server.OnDataReceive += (sender, message) => Console.WriteLine(message); // Message is automatically decrypted
```
```cs
using var encrypter = new EasyEncrypt("Password", "Salt531351235");
using var client = new EasyTcpClient().UseEncryption(encrypter); 
if(!client.Connect("127.0.0.1", PORT)) return;
client.Send("Hello encrypted server!"); // All data is automatically encrypted
```

## EasyTcp.Logging
EasyTcp.Logging adds support for logging of incoming/outgoing messages/connections and errors.
```cs
using var server = new EasyTcpServer().UseServerLogging(Console.WriteLine).Start(Port);

using var client = new EasyTcpClient().UseClientLogging(Console.WriteLine);
if(!client.Connect("127.0.0.1", Port)) return;
client.Send("Hello server!");
Console.ReadLine();      
```

# Contribution / Help / Questions / Feedback
[Join our matrix chat](https://matrix.to/#/!UfWuzAAgKkyPzNyPxM:matrix.org?via=matrix.org), create an issue or send an email to jobse@pm.me

# Thanks to
List with people who directly / indirectly contributed to this project.<br/>
@AndreiBlizorukov Fixed .gitignore<br/>
@Kermalis, @AbnirHencazs Reporting important issues<br/>
@Awware, @jchristn Inspiration<br/>

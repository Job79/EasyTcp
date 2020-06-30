<p align="center">
  <strong>EasyTcp</strong>
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
 <img src="https://img.shields.io/badge/version-3.5.0-green.svg">
  <img src="https://img.shields.io/badge/build-passing-green.svg">
  <br/>
  Easy and simple library for TCP clients and servers. Focused on performance and usability.
  <br/><br/><br/>
</p>

# EasyTcp
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

# EasyTcp.Actions
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

# EasyTcp.Encryption
EasyTcp.Encryption adds ssl adn encryption support to EasyTcp. <br/>
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
# Contribution / Help / Questions / Feedback
Create a issue, pull request or send an email to jobse@pm.me


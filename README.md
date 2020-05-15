<p align="center">
  <b>EasyTcp</b>
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img src="https://img.shields.io/badge/version-3.0.0.0-green.svg">
  <img src="https://img.shields.io/badge/build-passing-green.svg">
  <br/>
  <br/>
  <br/><br/>
</p>

# EasyTcp
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
            
if(!client.Connect(IPAddress.Loopback, PORT)) return; 
client.Send("Hello server");
```

# EasyTcp.Actions
```cs
public static void Run()
{
    using var server = new EasyTcpActionServer().Start(PORT);
    server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
    server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
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
client.SendAction("ECHO","Hello server");
client.SendAction("BROADCAST","Hello everyone"); 
```

# EasyTcp.Encryption
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

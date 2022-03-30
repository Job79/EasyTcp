# EasyTcp
<p>
  <img alt="MIT license" src="https://img.shields.io/badge/License-MIT-green.svg">
  <img alt="GitHub starts" src="https://img.shields.io/github/stars/job79/EasyTcp">
</p>

EasyTcp is a simple and fast tcp library that handles all the repetative tasks that are normally done when working with tcp.  
It tries to make tcp simple without giving up on functionality or performance.


## Different packages

EasyTcp has multiple packages, this to keep the project maintainable and the dependencies small. Every package has his own goal and all packages are compatible with eachother.

| Package            | Functionality                     |
|--------------------|-----------------------------------|
| EasyTcp            | Base package with the networking functionality. <br> Contains support for serialisation, multiple types of framing, compression, logging, streaming and disconnect detection (with optional keep alive) |
| EasyTcp.Actions    | Support for EasyTcp to triggering specific functions with an attribute based on received data |
| EasyTcp.Encryption | TLS/SSL support for EasyTcp and EasyTcp.Actions |

### Downloads
```
dotnet add package EasyTcp
dotnet add package EasyTcp.Actions
dotnet add package EasyTcp.Encryption
```

### Documentation

Work in progress...

## Examples

### EasyTcp
```cs
using var server = new EasyTcpServer().Start(8080);
server.OnDataReceive += (sender, message) => Console.WriteLine(message);

using var client = new EasyTcpClient();
if(!client.Connect("127.0.0.1", 8080)) return; // Abort if connection attempt failed
client.Send("Hello server");
Console.ReadLine();

// Output: Hello server
```

### EasyTcp.Actions
```cs
static void Main(string[] args)
{
    using var server = new EasyTcpActionServer().Start(8080); // Server automatically detects all action methods within the current assembly
    server.OnUnknownAction += (s, actionMessage) => Console.WriteLine("Unknown action received");
    
    using var client = new EasyTcpClient();
    if(!client.Connect("127.0.0.1", PORT)) return; 
    client.SendAction("ECHO","Hello me"); // Trigger the ECHO action server side
    client.SendAction("BROADCAST","Hello everyone"); // Trigger the BROADCAST action server side
    Console.ReadLine();
}

[EasyAction("ECHO")] // Trigger function when "ECHO" action is received
public void EchoAction(Message message)
{
    message.Client.Send(message);
}

[EasyAction("BROADCAST")]
public void BroadcastAction(object sender, Message message)
{
    var server = (EasyTcpServer) sender; // Retrieve server
    server.SendAll(message);
}
```

### EasyTcp.Encryption
```cs
var certificate = new X509Certificate2("certificate.pfx", "password"); // Load ssl certificate
using var server = new EasyTcpServer().UseServerSsl(certificate).Start(8080); // Use ssl for all incoming / outgoing messages
server.OnDataReceive += (sender, message) => Console.WriteLine(message); // Message is automatically decrypted

using var client = new EasyTcpClient().UseSsl("localhost",  acceptInvalidCertificates: true); // "localhost" = server domain
if(!client.Connect("127.0.0.1", 8080)) return;
client.Send("Hello ssl server!"); // All data is automatically encrypted
Console.ReadLine();
```

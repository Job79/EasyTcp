<p align="center">
  <img src="icon.png" width="300px">
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img alt="Nuget" src="https://img.shields.io/nuget/v/EasyTcp">
  <img alt="Nuget" src="https://img.shields.io/nuget/dt/EasyTcp">
  <img alt="GitHub stars" src="https://img.shields.io/github/stars/job79/EasyTcp">
  <br/>
  Easy and simple library for TCP clients and servers. Focused on performance and usability.
  <br/><br/><br/>
</p>

## EasyTcp?
EasyTcp is a library that makes creating tcp servers and clients simple without giving up performance* <br/> 
It has build in serialisation, compression, different types of framing and an (optional) async interface.

\* ~400.000 round trips/second, tested on local machine(linux, ryzen 7) with clients and server running under the same process. See EasyTcp.Examples/SpeedTest/ThroughputTest.cs

## Example
The folowing code will create a tcp server that writes all received data to the terminal. <br/>
Then it will create a client that connects with the server followed by the client sending: "Hello Server"
```cs
using var server = new EasyTcpServer().Start(8080);
server.OnDataReceive += (sender, message) => Console.WriteLine(message);

using var client = new EasyTcpClient();
if(!client.Connect("127.0.0.1", 8080)) return; // Abort if connection attempt failed
client.Send("Hello server");
Console.ReadLine();

// Output terminal: "Hello server"
```
See also [EasyTcp/Examples](https://github.com/Job79/EasyTcp/tree/master/EasyTcp3/EasyTcp3.Examples)

## Different packages
EasyTcp has different packages, this to keep the project maintainable and the dependencies small (in disk size).
Every package has his own goal and all packages are compatible with eachother.

### EasyTcp
EasyTcp is the package that contains all the basic functions for both servers and clients. <br/>
It includes multiple types of framing, disconnect detection, compression, serialisation, event handlers, streaming support and much more.

## EasyTcp.Actions
EasyTcp.Actions adds support to EasyTcp for triggering functions based on received data for both client and servers. <br/>
It does this without giving up (noticeable) performance, and makes creating big servers/clients easy/maintainable. <br/>
Here is a very basic code example demonstrating EasyTcp.Actions:
```cs
static void Main(string[] args)
{
     using var server = new EasyTcpActionServer().Start(PORT); // Server automatically detects all action methods within the current assembly
     server.OnUnknownAction += (s, actionMessage) => Console.WriteLine("Unknown action received");
     Console.ReadLine();
}

[EasyTcpAction("ECHO")] // Trigger function when "ECHO" action is received
public void EchoAction(Message message)
{
    message.Client.Send(message);
}

[EasyTcpAction("BROADCAST")]
public void BroadCastAction(object sender, Message message)
{
    var server = (EasyTcpServer) sender; // Retrieve server
    server.SendAll(message);
}
```

```cs
using var client = new EasyTcpClient();
if(!client.Connect("127.0.0.1", PORT)) return; 
client.SendAction("ECHO","Hello me"); // Trigger the ECHO action server side
client.SendAction("BROADCAST","Hello everyone"); // Trigger the BROADCAST action server side
Console.ReadLine();
```

## EasyTcp.Encryption
EasyTcp.Encryption adds tls/ssl and custom encryption support to EasyTcp. <br/>
Here's another basic code example:
```cs
using var certificate = new X509Certificate2("certificate.pfx", "password"); // Load ssl certificate
using var server = new EasyTcpServer().UseSsl(certificate).Start(PORT); // Use ssl for all incoming / outgoing messages

server.OnDataReceive += (sender, message) => Console.WriteLine(message); // Message is automatically decrypted
```
```cs
using var client = new EasyTcpClient().UseSsl("localhost",  acceptInvalidCertificates: true); // "localhost" = server domain
if(!client.Connect("127.0.0.1", PORT)) return;
client.Send("Hello ssl server!"); // All data is automatically encrypted
```
<br/><br/>
```cs
using var encrypter = new EasyEncrypt("Password", "Salt531351235"); // Encryption library used by EasyTcp, default = AES encryption. 
using var server = new EasyTcpServer().UseEncryption(encrypter).Start(PORT);  // Use encryption for all incoming / outgoing messages

server.OnDataReceive += (sender, message) => Console.WriteLine(message); // Message is automatically decrypted
```
```cs
using var encrypter = new EasyEncrypt("Password", "Salt531351235");
using var client = new EasyTcpClient().UseEncryption(encrypter); 
if(!client.Connect("127.0.0.1", PORT)) return;
client.Send("Hello encrypted server!"); // All data is automatically encrypted
```

## EasyTcp.Logging
EasyTcp.Logging adds support for logging of incoming/outgoing messages/connections and internal errors.
```cs
using var server = new EasyTcpServer().UseServerLogging(Console.WriteLine).Start(Port);

using var client = new EasyTcpClient().UseClientLogging(Console.WriteLine);
if(!client.Connect("127.0.0.1", Port)) return;
client.Send("Hello server!");
Console.ReadLine();      
```

# Contribution / Help / Questions / Feedback
Create an issue or send an email to jobse@pm.me

# Thanks to
List with people who directly / indirectly contributed to this project.<br/>
@AndreiBlizorukov Fixed .gitignore<br/>
@Kermalis, @AbnirHencazs Reporting important issues<br/>
@Awware, @jchristn Inspiration<br/>

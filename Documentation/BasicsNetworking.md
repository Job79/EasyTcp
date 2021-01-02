# The basics of networking

## What we need for communication between 2 machines

1. We need a physical connection between the 2 computers.  
   Most of the time there is another device between the 2 computers, more about that later.
2. A common language, also known as protocol.
3. The client (the one opening the connection) needs the address of the server.

### The protocol

There are multiple networking protocols. The most well known one's are TCP/IP and UDP. Both have some pros and cons.  
UDP is faster than TCP, but does not guarantee that the message is actually correctly delivered.  
TCP is a little slower, but does guarantee that the message gets correctly delivered.  

Which one is the best? It really depends on the situation, if it doesn't matter that sometimes some data is dropped and performance is important, UDP is probably the best option. 
Examples of situations where data can be dropped: streaming a video or audio, communication the location of players in a game.  

EasyTcp currently only supports TCP, but we may add support for UDP in the future.  
More information about the difference between network protocols can be found here: [1](https://www.colocationamerica.com/blog/tcp-ip-vs-udp), [2](https://en.wikipedia.org/wiki/Transmission_Control_Protocol), [3](https://en.wikipedia.org/wiki/User_Datagram_Protocol)

### (Local) IPAddress
Before a connection can be established we need the IP address of another computer.  
The address of a computer can be found by executing 1 of the following commands:  
**Linux**  `ip a | awk '/inet/ { print $2 }'`  
**Bsd**  `ifconfig | awk '/inet/ { print $2 }'`  
**Windows** `ipconfig` + search for IPv4/IPv6 address  

There are 2 different "versions" of an ip address. This because the IPv4 protocol didn't have enough addresses available for all our devices.  
Example IPv4 address: "192.168.1.181"  
Example IPv6 address: "fe80::329c:23ff:fe06:84ec"  
EasyTcp works with both IPv4 and IPv6, but the server .Start() method defaults to IPv4.  

## Creating a connection & ports
For a connection we need a server and a client.  
The client is the one starting the connection, the server accepts the connection of the client.  
A client can only connected to 1 server, a server can accept multiple clients.  

A server is started on a port, a port is a small number(ushort 0-65535) that is used to give the received information to the right process (or computer when behind a router).  
When a server is started on a port < 1024, it needs to run as root (or admin privileges on windows)  
A client needs the IP and the port of a server before it can connect.  

Establishing a connection with EasyTcp:  
```cs
using var server = new EasyTcpServer().Start(8080); // Start server on port 8080

using var client = new EasyTcpClient();
client.Connect("192.168.1.128", 8080); // Connect to server with address "192.168.1.128" and port 8080
```

## NAT, router and port forwarding
Most devices are connected to a router, which handles the traffic between the internet and the connected devices.  
So a router is a "gateway" or "door" to the internet. And this "gateway" has 2 sides, the external(WAN) and internal(LAN) side.  
The router hides the internal ip addresses from the internet. All the connected devices get 1 shared ip address.  
This ip address is different from a local one.  
See your own external ip address by visiting `http://api.ipify.org/` or `api6.ipify.org`, or by executing `curl api64.ipify.org`  

Someone can't connect to your local server before you "port forward" your router.  
This will tell the router that once someone tries to connect to a specific port it is meant for the server with local ip address x.x.x.x  
Go to the website of your router(local ip, probably 192.168.1.1 or 192.168.1.254) and try port forwarding it there, this is different for every router

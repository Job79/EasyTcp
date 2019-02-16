# EasyTcp
A high performance/async tcp server and client. (SocketBased) (Supports IPv6 & IPv4) Simple and easy to use(but also advanced) and has a good documentation. https://www.nuget.org/packages/EasyTcp/

# Performance
Because EasyTcp is socket based and async it has very good performance.
I write a test to see the performance:
<details>
  <summary>Click to see test code</summary>
  
  Client:
```cs
const int PORT = 1000;
const int MESSAGES_COUNT = 1000000;
const string MESSAGE = "Message";

void SpeedTest()
{
    EasyTcpClient Client = new EasyTcpClient();

    if (Client.Connect(IPAddress.Loopback, PORT, TimeSpan.FromSeconds(1))) Console.WriteLine("Client connected");
    else { Console.WriteLine("Could not connect"); Console.ReadKey(); return; }

    byte[] Message = Encoding.UTF8.GetBytes(MESSAGE);
    Stopwatch sw = new Stopwatch();
    sw.Start();

    for (int x = 0; x < MESSAGES_COUNT; x++) { Client.SendAndGetReply(Message, TimeSpan.FromSeconds(1)); }

    sw.Stop();
    Console.WriteLine($"ElapsedMilliseconds SpeedTest: {sw.ElapsedMilliseconds}");
    Console.WriteLine($"Average SpeedTest: {sw.ElapsedMilliseconds / MESSAGES_COUNT}");
}
```
Server:
```cs
const int PORT = 1000;

static void Main(string[] args)
{
    EasyTcpServer Server = new EasyTcpServer();
    Server.DataReceived += (object sender, Message e) => e.Reply(e.Data);
    Server.Start(IPAddress.Any, PORT, 1000);

    Task.Delay(-1).Wait();
}
```
</details>

<details>
  <summary>Output</summary>
  Test1 = 56923ms
  56923ms / 1.000.000 messages = 0.56923ms
  
  Test2 = 58287ms
  58287 / 1.000.000 messages = 0.58287ms
  
  Test3 = 58577ms
  58577ms / 1.000.000 messages = 0.58577ms
  
  Test4 = 58708ms
  58708ms / 1.000.000 messages = 0.58708ms
  
  Test 5 = 59209ms
  58708ms / 1.000.000 messages = 0.59209ms
  
  0.56923 + 0.58287 + 0.58577 + 0.58708 + 0.59209 = 2.91414ms
  Average = 2.91414 / 5 = 0.582828ms
</details>

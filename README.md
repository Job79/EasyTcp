<p align="center">
  <b>EasyTcp</b>
  <br/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img src="https://img.shields.io/badge/version-2.0.4.1-green.svg">
  <img src="https://img.shields.io/badge/build-passing-green.svg">
  <br/>
  <br/>
  <a>A high performance tcp server and client. (Async) (IPv6 & IPv4)<br/>Simple and easy to use.<a/>
  <br/><br/>
</p>

# Performance
Because EasyTcp is socket based and async it has good performance.
I made a test to see the performance.
This test will send a message to the server, then the server send the same message back.
If the client received the message it sends the next one.
The average of this test: 0.0582828ms (on localhost)
<details>
  <summary>Click to see test code</summary>
  
  Client:
```cs
    const int Port = 1000;
    const int MessageCount = 1000000;
    const string Message = "Message";

    void SpeedTest()
    {
        EasyTcpClient client = new EasyTcpClient();

        if (client.Connect(IPAddress.Loopback, Port, TimeSpan.FromSeconds(1))) Console.WriteLine("Client connected");
        else { Console.WriteLine("Could not connect"); Console.ReadKey(); return; }

        byte[] message = Encoding.UTF8.GetBytes(Message);
        Stopwatch sw = new Stopwatch();
        sw.Start();

        for (int x = 0; x < MessageCount; x++) { client.SendAndGetReply(message, TimeSpan.FromSeconds(1)); }

        sw.Stop();
        Console.WriteLine($"ElapsedMilliseconds SpeedTest: {sw.ElapsedMilliseconds}");
        Console.WriteLine($"Average SpeedTest: {sw.ElapsedMilliseconds / (decimal)MessageCount}");
    }
```
Server:
```cs
    const int Port = 1000;

    static void Main(string[] args)
    {
        EasyTcpServer server = new EasyTcpServer();
        server.DataReceived += (object sender, Message e) => e.Reply(e.Data);
        server.Start(IPAddress.Any, Port, 1000);

        Task.Delay(-1).Wait();
    }
```
</details>

<details>
  <summary>Click to see output</summary>
  Test1 = 56923ms
  
  56923ms / 1.000.000 messages = 0.056923ms
  
  Test2 = 58287ms  
  58287 / 1.000.000 messages = 0.058287ms
  
  Test3 = 58577ms  
  58577ms / 1.000.000 messages = 0.058577ms
  
  Test4 = 58708ms  
  58708ms / 1.000.000 messages = 0.058708ms
  
  Test 5 = 59209ms  
  58708ms / 1.000.000 messages = 0.059209ms
 
  0.056923 + 0.058287 + 0.058577 + 0.058708 + 0.059209 = 0.291414ms  
  Average = 0.291414 / 5 = 0.0582828ms
</details>

# [Documentation](https://github.com/GHenkje/EasyTcp/wiki)

using System;
using System.IO;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Examples.Streams
{
    /* Example usage of the SendStream and ReceiveStream functions (Also see file server/client)
     * Use these functions when sending extreme large data.
     */
    public class StreamExample
    {
        private const ushort Port = 6179;
        public static void Run()
        {
            var server = new EasyTcpActionServer().Start(Port);
        }
        
        public static void Connect()
        {
            using var client = new EasyTcpClient();
            if(!client.Connect("127.0.0.1", Port)) return;
            
            // Trigger action
            client.SendAction("ReceiveStream");
           
            // Send large array
            // Length of stream is prefixed by default (long as byte[8])
            var stream = new MemoryStream(new byte[100000]);
            client.SendStream(stream);
            
            // Send stream without length prefix
            var stream2 = new MemoryStream(new byte[10000]);
            client.SendStream(stream2, sendLengthPrefix: false);
            Console.ReadLine();

            // Writing / reading the base stream is also possible
            var baseStream = client.Protocol.GetStream(client);
        }

        /*
         * Receive is only working in OnDataReceive or action methods
         * This because the internal dataReceiver will read the stream when running 
         */
        [EasyTcpAction("ReceiveStream")]
        public async Task ReceiveStream(Message message)
        {
            // Receive stream with prefix
            await using var stream = new MemoryStream();
            await message.ReceiveStreamAsync(stream);
            Console.WriteLine($"Received {stream.Length} bytes");

            // Receive stream with known length 
            await using var stream2 = new MemoryStream();
            await message.ReceiveStreamAsync(stream2, count: 10000);
            Console.WriteLine($"Received {stream2.Length} bytes");
        }
    }
}
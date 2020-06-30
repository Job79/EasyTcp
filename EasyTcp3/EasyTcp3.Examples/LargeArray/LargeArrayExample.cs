using System;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.LargeArray
{
    /* Example usage of the SendLargeArray and ReceiveLargeArray functions
     * Use these functions when sending large data (over 65.535 bytes)
     * See stream examples for extreme large data
     */
    public class LargeArrayExample
    {
        private const ushort Port = 6171;

        public static void Run()
        {
            var server = new EasyTcpActionServer().Start(Port);
        }

        public static void Connect()
        {
            using var client = new EasyTcpClient();
            if (!client.Connect("127.0.0.1", Port)) return;

            // Trigger action
            client.SendAction("LargeArray");

            // Send large array
            // Length of array is prefixed by default (int as byte[4])
            client.SendLargeArray(new byte[1000000]);

            // Send large array without length prefix
            client.SendLargeArray(new byte[10000], false);
            Console.ReadLine();
        }

        /*
         * Receive is only working in OnDataReceive or action methods
         * This because the internal dataReceiver will read the stream when running 
         */
        [EasyTcpAction("LargeArray")]
        public async Task LargeArrayReceive(Message message)
        {
            // Receive array with prefix
            var largeArray = await message.ReceiveLargeArrayAsync();
            Console.WriteLine($"Received {largeArray.Length} bytes");

            // Receive array with known length 
            var largeArray2 = await message.ReceiveLargeArrayAsync(10000);
            Console.WriteLine($"Received {largeArray2.Length} bytes");
        }
    }
}
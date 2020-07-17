using System;
using System.Numerics;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Examples.Serialization
{
    /* Custom IEasyTcpPacket
     * Data { get; set; } is used instead of standard serialisation
     */
    public class ExamplePacket : IEasyTcpPacket
    {
        public BigInteger Content
        {
            get => new BigInteger(Data);
            set => Data = value.ToByteArray();
        }

        public byte[] Data { get; set; }
    }

    public static class CustomPacketExample
    {
        private const int Port = 1239;
        
        public static void Connect()
        {
            using var client = new EasyTcpClient();
            if(!client.Connect("127.0.0.1", Port)) return;

            /* Send custom packet,
             * Data { get; } is used instead of serialisation if class implements IEasyTcpPacket
             */
            client.Send(new ExamplePacket {Content = 100});
            
            // Received message to custom EasyTcpPacket
            client.OnDataReceive += (s, message) =>
            {
                var customPacket = message.ToPacket<ExamplePacket>();
                Console.WriteLine(customPacket.Content);
            };
        }
    }
}
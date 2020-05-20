using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.CustomPacket
{
    /// <summary>
    /// Example usage of EasyTcpPacket
    /// </summary>
    public static class EasyTcpPacketExample
    {
        private const ushort Port = 3214;
        
        public static void Start()
        {
            var server  = new EasyTcpServer().Start(3214);
            server.OnDataReceive += (sender, message) =>
            {
                foreach (var i in message.ToPacket<PacketList<Message>>()) // Convert message to list
                    Console.WriteLine(i.ToString());
            };
        }
        
        public static void Connect()
        {
            var client = new EasyTcpClient();
            if(!client.Connect(IPAddress.Loopback, Port)) return; 
            
            var list = new PacketList<Message>
            {
                EasyTcpPacket.To<Message>("Message1"),
                EasyTcpPacket.To<Message>("Message2"),
                EasyTcpPacket.To<Message>("Message3")
            }; 
            client.Send(list); // Send list
            
            // Message also implements IEasyTcpPacket
            // So we can use all functions on message as example
            // This is how we create a new message with as content a string
            // The EasyTcpPacket.To can be used on all packets, but not all packets support being created from a string
            // Our list for example wouldn't work when created from a string 
            var message = EasyTcpPacket.To<Message>("S_T_R_I_N_G");
            message.Compress(); // Compress message
        }
    }

    /*
     * Our custom tcp packet,
     * a custom List<IEasyTcpPacket> that can be send over the internet.
     *
     * All implementations of IEasyTcpPacket must have a `public byte[] Data{ get; set; };`
     *
     * This implementation does not support compression and encryption because it will become invalid and throw an exception 
     */
    class PacketList<T> : List<T>, IEasyTcpPacket where T : IEasyTcpPacket, new()
    {
        private byte[] _data;
        public byte[] Data {
            get { UpdateData(); return _data; }
            set { _data = value; UpdateList(); } 
        }

        private void UpdateData()
        {
            int position = 0;
            _data = new byte[this.Sum(x=>x.Data.Length + 4)];
            foreach (var data in this.Select(m=>m.Data))
            {
                Buffer.BlockCopy(BitConverter.GetBytes(data.Length),0,_data,position,4);
                Buffer.BlockCopy(data,0,_data, position + 4,data.Length);
                position += data.Length + 4;
            } 
        }

        private void UpdateList()
        {
            Clear();
            int position = 0;
            while(position < _data.Length) 
            {
                int length = BitConverter.ToInt32(_data, position);
                if(length <= 0 || length + position > _data.Length) throw new Exception("List is invalid");
                Add( EasyTcpPacket.To<T>(_data[(position+=4)..(position+=length)]));
            }
        }
    }
}
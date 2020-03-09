using System.Net.Sockets;
using EasyTcp3.Client;

namespace EasyTcp3
{
    public class Message
    {
        public byte[] Data;
        public EasyTcpClient Socket;

        public Message(byte[] data, EasyTcpClient socket)
        {
            Data = data;
            Socket = socket;
        }
    }
}
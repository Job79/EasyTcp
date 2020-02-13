using System.Net.Sockets;
using System.Text;

namespace EasyTcp.Core
{
    public class Message
    {
        public byte[] Data;
        public Encoding Encoding;
        public Socket Socket;

        public Message(byte[] data,Socket socket, Encoding encoding)
        {
            Data = data;
            Socket = socket;
            Encoding = encoding;
        }
    }
}
using System.IO;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Files
{
    /// <summary>
    /// This class contains an example of how to make a basic file server
    /// </summary>
    public static class FileServer
    {
        private const ushort Port = 5_002;

        public static void StartFileServer()
        {
            new EasyTcpActionServer().Start(Port);
        }

        [EasyTcpAction("DOWNLOAD")]
        public static void Download(object s, Message m)
        {
            using var fileStream = new FileStream(m.ToString(), FileMode.Open);
            m.Client.Send("Uploading file!");
            m.Client.SendStream(fileStream); // Send stream and use fileStream as source
        }

        [EasyTcpAction("UPLOAD")]
        public static void Upload(object s, Message m)
        {
            using var fileStream = new FileStream(m.ToString(), FileMode.Create);
            m.ReceiveStream(fileStream); // Receive stream and write receiving stream to fileStream
        }
    }
}
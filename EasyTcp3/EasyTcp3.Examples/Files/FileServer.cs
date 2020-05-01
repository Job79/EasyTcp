using System.IO;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
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
            var server = new EasyTcpServer();
            server.Start(Port);
            server.OnDataReceive += OnDataReceive;
        }

        private static void OnDataReceive(object sender, Message e)
        {
            string message = e.ToString();
            
            if (message.StartsWith("Download ")) // Client wants to download a file
            {
                string file = message.Remove(0,9); // Remove "Download "
                
                using var fileStream = new FileStream(file, FileMode.Open);
                e.Client.Send("Uploading file!");
                e.Client.SendStream(fileStream);
            }
            else if (message.StartsWith("Upload ")) // Client wants to upload a file
            {
                string file = message.Remove(0,7); // Remove "Upload "
                
                using var fileStream = new FileStream(file, FileMode.Create);
                e.ReceiveStream(fileStream);
            }
        }
    }
}
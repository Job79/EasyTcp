using System;
using System.IO;
using System.Net;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Examples.Files
{
    /// <summary>
    /// Example of a client that uploads and downloads files from/to a server
    /// </summary>
    public static class FileClient
    {
        private const ushort Port = 5_002;

        public static void Download(string fileName, string saveAs)
        {
            var client = new EasyTcpClient();
            client.OnDataReceive += (sender, message) =>
            {
                using var fileStream = new FileStream(saveAs, FileMode.Create);
                message.ReceiveStream(fileStream);
                Console.WriteLine($"Downloaded {fileName}, saved as {saveAs}");
            };
            
            if(!client.Connect(IPAddress.Any, Port)) return;
            client.Send($"Download {fileName}");
        }
        
        public static void Upload(string fileName, string saveAs)
        {
            var client = new EasyTcpClient();

            if(!client.Connect(IPAddress.Any, Port)) return;
            client.Send($"Upload {saveAs}");
            
            using var fileStream = new FileStream(fileName, FileMode.Open);
            client.SendStream(fileStream);
            Console.WriteLine($"Uploaded {fileName}, saved as {saveAs}");
        }
    }
}
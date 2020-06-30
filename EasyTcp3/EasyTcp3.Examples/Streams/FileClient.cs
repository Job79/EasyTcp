using System;
using System.IO;
using System.Net;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Examples.Streams
{
    /* Example client that sends/receives a stream(file) over the network
     */
    public static class FileClient
    {
        private const ushort Port = 5_002;

        public static void Download(string fileName, string saveAs)
        {
            using var client = new EasyTcpClient();
            client.OnDataReceive += (sender, message) =>
            {
                using var fileStream = new FileStream(saveAs, FileMode.Create);
                message.ReceiveStream(fileStream); // Receive stream and write receiving stream to fileStream
                Console.WriteLine($"Downloaded {fileName}, saved as {saveAs}");
            };

            if (!client.Connect(IPAddress.Any, Port)) return;
            client.SendAction("DOWNLOAD", fileName);
        }

        public static void Upload(string fileName, string saveAs)
        {
            using var client = new EasyTcpClient();

            if (!client.Connect(IPAddress.Any, Port)) return;
            client.SendAction("UPLOAD", saveAs);

            using var fileStream = new FileStream(fileName, FileMode.Open);
            client.SendStream(fileStream); // Send stream and use fileStream as source
            Console.WriteLine($"Uploaded {fileName}, saved as {saveAs}");
        }
    }
}
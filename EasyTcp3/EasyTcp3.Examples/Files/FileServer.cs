using System.IO;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Files
{
    /// <summary>
    /// This class contains an example of how to make a basic file server
    /// </summary>
    public class FileServer
    {
        private const ushort Port = 5_002;

        public static void StartFileServer()
        {
            new EasyTcpActionServer().Start(Port);
        }

        [EasyTcpAction("DOWNLOAD")]
        public async Task Download(Message m)
        {
            await using var fileStream = new FileStream(m.ToString(), FileMode.Open);
            m.Client.Send("Uploading file!");
            await m.Client.SendStreamAsync(fileStream); // Send stream and use fileStream as source
        }

        [EasyTcpAction("UPLOAD")]
        public async Task Upload(Message m)
        {
            await using var fileStream = new FileStream(m.ToString(), FileMode.Create);
            await m.ReceiveStreamAsync(fileStream); // Receive stream and write receiving stream to fileStream
        }
    }
}
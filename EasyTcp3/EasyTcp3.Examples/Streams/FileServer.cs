using System.IO;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils.Async;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Streams
{
    /* Example server that sends/receives a stream(file) over the network
     */
    public class FileServer
    {
        private const ushort Port = 5_002;

        public static void StartFileServer() => new EasyTcpActionServer().Start(Port);

        [EasyTcpAction("DOWNLOAD")]
        public async Task Download(Message m)
        {
            await using var fileStream = new FileStream(m.ToString(), FileMode.Open);
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
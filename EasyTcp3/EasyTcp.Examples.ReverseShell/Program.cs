using EasyTcp3.Examples.ReverseShell.Client;
using EasyTcp3.Examples.ReverseShell.Server;

namespace EasyTcp.Examples.ReverseShell
{
    /// <summary>
    /// Example application with easyTcp.Actions (ReverseShell)
    ///
    /// Commands:
    /// s | select                        Select client
    /// execute [application]             Start application on selected client machine (For terminal: bash|zsh|fish|dash etc.. for linux, cmd|powershell for windows)
    /// > [input]                         Pipe input into the stdin of the running application
    ///
    /// ping                              Send ping to client, client will respond with "pong!"
    /// echo [input]                      Send input to client, client will echo data back to server
    /// print [input]                     Write input into stdout of current running instance of client
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            ushort port = 52525;
            var client = new ReverseShellClient();
            var server = new ReverseShellServer();
            var x = client.Start(ip, port);
            server.Start(port);
        }
    }
}
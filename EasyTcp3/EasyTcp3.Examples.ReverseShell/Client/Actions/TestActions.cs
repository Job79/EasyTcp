using System;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;

namespace EasyTcp3.Examples.ReverseShell.Client.Actions
{
    /// <summary>
    /// Some simple actions to test the reverse shell
    /// </summary>
    public static class TestActions
    {
        [EasyTcpAction("print")]
        public static void Print(object s, Message e)
            => Console.WriteLine(e.Decompress().ToString());

        [EasyTcpAction("echo")]
        public static void Echo(object s, Message e)
            => e.Client.Send(e);

        [EasyTcpAction("ping")]
        public static void Ping(object s, Message e)
            => e.Client.Send("pong!");
    }
}
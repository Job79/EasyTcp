using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Test.Actions
{
    public static class Actions
    {
        [EasyTcpAction(0)]
        public static void Echo(object s, Message e)
            => e.Client.Send(e.Data);
        
        [EasyTcpAction(1)]
        public static void EchoAction(object s, Message e)
            => e.Client.SendAction(1,e.Data);
    }
}
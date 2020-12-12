using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Actions used by multiple tests
    /// </summary>
    public class Actions
    {
        [EasyTcpAction(0)]
        public void Echo(Message e)
            => e.Client.Send(e);
        
        [EasyTcpAction("ECHO")]
        public static void Echo2(object s, Message e)
            => e.Client.Send(e);
    }
}

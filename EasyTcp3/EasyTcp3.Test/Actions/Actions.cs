using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Test.Actions
{
    public class Actions
    {
        [EasyTcpAction(0)]
        public async Task Echo(Message e)
        {
            e.Client.Send(e);
        }


        [EasyTcpAction("ECHO")]
        public static async Task Echo2(object s, Message e)
            => e.Client.Send(e);
    }
}
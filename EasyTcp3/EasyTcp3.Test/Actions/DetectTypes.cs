using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Test.Actions.Types;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    public class DetectTypes
    {
        [Test]
        public async Task RunActionsWithDifferentTypes()
        {
            for(var type = 1; type < 7; type++)
            {
                TestActions.Counter = 0;
                var client = new EasyTcpActionClient(nameSpace: "EasyTcp3.Test.Actions.Types");
                var server = new EasyTcpActionServer(nameSpace: "EasyTcp3.Test.Actions.Types");

                await client.ExecuteAction(type);
                await server.ExecuteAction(type);

                Assert.AreEqual(2, TestActions.Counter);
            }
        }
    }
}

namespace EasyTcp3.Test.Actions.Types
{
    public class TestActions
    {
        public static int Counter;

        [EasyTcpAction(1)]
        public void One(object sender, Message message)
            => Counter++;

        [EasyTcpAction(2)]
        public void Two(Message message)
            => Counter++;

        [EasyTcpAction(3)]
        public void Three()
            => Counter++;

        [EasyTcpAction(4)]
        public static void StaticFour()
            => Counter++;

        [EasyTcpAction(5)]
        public async Task Five(object sender, Message message)
            => Counter++;

        [EasyTcpAction(6)]
        public async Task Six(Message message)
            => Counter++;

        [EasyTcpAction(7)]
        public async Task Seven()
            => Counter++;
    }
}

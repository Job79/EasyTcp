using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Test.Actions.Types;
using NUnit.Framework;

namespace EasyTcp3.Test.Actions
{
    /// <summary>
    /// Test that determines whether all delegate types are successfully detected
    /// </summary>
    public class DetectAllTypes
    {
        [Test]
        public void TestDetectAllTypes()
        {
            var client = new EasyTcpActionClient(nameSpace: "EasyTcp3.Test.Actions.Types");
            for (int i = 1; i <= 7; i++)
            {
                client.ExecuteAction(i);
                Assert.AreEqual(i, TestActions.Counter);
            }

            var server = new EasyTcpActionServer(nameSpace: "EasyTcp3.Test.Actions.Types");
            TestActions.Counter = 0;
            for (int i = 1; i <= 7; i++)
            {
                server.ExecuteAction(i);
                Assert.AreEqual(i, TestActions.Counter);
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
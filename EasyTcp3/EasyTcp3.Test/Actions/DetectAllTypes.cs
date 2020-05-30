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
                Assert.AreEqual(i, TestActions.counter);
            }

            var server = new EasyTcpActionServer(nameSpace: "EasyTcp3.Test.Actions.Types");
            TestActions.counter = 0;
            for (int i = 1; i <= 7; i++)
            {
                client.ExecuteAction(i);
                Assert.AreEqual(i, TestActions.counter);
            }
        }
    }
}

namespace EasyTcp3.Test.Actions.Types
{
    public class TestActions
    {
        public static int counter;

        [EasyTcpAction(1)]
        public void One(object sender, Message message)
            => counter++;

        [EasyTcpAction(2)]
        public void Two(Message message)
            => counter++;

        [EasyTcpAction(3)]
        public void Three()
            => counter++;

        [EasyTcpAction(4)]
        public static void StaticFour()
            => counter++;

        [EasyTcpAction(5)]
        public async Task Five(object sender, Message message)
            => counter++;

        [EasyTcpAction(6)]
        public async Task Six(Message message)
            => counter++;

        [EasyTcpAction(7)]
        public async Task Seven()
            => counter++;
    }
}
using System.Threading.Tasks;
using EasyTcp4.Actions;
using EasyTcp4;
using NUnit.Framework;
using DetectTestActions;

namespace EasyTcp4.Test.Actions
{
    public class Detection
    {
        [Test]
        public void DetectAllTypes()
        {
            var client = new EasyTcpActionClient(nameSpace: "DetectTestActions");
            Assert.AreEqual(7, client.Actions.Count);
        }

        [Test]
        public async Task DetectAndExecuteTypes()
        {
            var server = new EasyTcpActionServer(nameSpace: "DetectTestActions");
            for (int i = 1; i <= 7; i++) await server.ExecuteAction(i);
            Assert.AreEqual(7, TestActions.Counter);
        }
    }
}

namespace DetectTestActions
{
    public class TestActions
    {
        public static int Counter;

        [EasyAction(1)]
        public void One(object sender, Message message) => Counter++;

        [EasyAction(2)]
        public void Two(Message message) => Counter++;

        [EasyAction(3)]
        public void Three() => Counter++;

        [EasyAction(4)]
        public static void StaticFour() => Counter++;

        [EasyAction(5)]
        public Task Five(object sender, Message message)
        {
            Counter++;
            return Task.CompletedTask;
        }

        [EasyAction(6)]
        public Task Six(Message message)
        {
            Counter++;
            return Task.CompletedTask;
        }

        [EasyAction(7)]
        public Task Seven()
        {
            Counter++;
            return Task.CompletedTask;
        }
    }
}

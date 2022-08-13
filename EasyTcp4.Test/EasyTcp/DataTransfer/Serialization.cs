using System.Threading;
using System.Threading.Tasks;
using EasyTcp4.ClientUtils;
using NUnit.Framework;

namespace EasyTcp4.Test.EasyTcp.DataTransfer
{
    public class Serialization
    {
        [Test]
        public async Task SendSerialisedArray()
        {
            using var conn = await TestHelper.GetTestConnection();

            char[] items = { 't', 'e', 's', 't', ' ', 'a', 'r', 'r', 'a', 'y' };
            int receivedItems = 0;
            conn.Server.OnDataReceive += (_, m) => Interlocked.Add(ref receivedItems, m.To<char[]>().Length);
            conn.Client.Send(items);

            await TestHelper.WaitWhileFalse(() => receivedItems == items.Length);
            Assert.AreEqual(items.Length, receivedItems);
        }
    }
}

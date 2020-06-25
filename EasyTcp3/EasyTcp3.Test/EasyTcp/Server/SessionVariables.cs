using NUnit.Framework;

namespace EasyTcp3.Test.EasyTcp.Server
{
    public class SessionVariables
    {
        [Test]
        public void TestVariables()
        {
            using var client = new EasyTcpClient();
            client.Session["client"] = client;
            
            Assert.AreEqual(1, client.Session.Count);
            Assert.AreEqual(client, (EasyTcpClient)client.Session["client"]);
        }
    }
}
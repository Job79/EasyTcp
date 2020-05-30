using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;

namespace EasyTcp3.Examples.Actions
{
    /// <summary>
    /// this class contains examples of the Action utils 
    /// </summary>
    public class ActionClient
    {
        private const ushort Port = 6_001;

        public static void Connect()
        {
            var client = new EasyTcpClient();
            if (!client.Connect("127.0.0.1", Port)) return;

            // All actions are send as an int, this is the action id of the "ECHO" action.
            // Integers can also be used as action codes. However this is not very readable.
            // Because everything is send as an int collisions are possible. (The algorithm used is named djb2a [http://www.cse.yorku.ca/~oz/hash.html])
            // This is no problem because this is very rare, however keep it in mind!
            // Try for example these two: haggadot & loathsomenesses
            int actionCode = "ECHO".ToActionCode();

            // Execute the "ECHO" action on our echo server
            client.SendAction("ECHO", "Hello me!");

            // Execute the "BROADCAST" action on our echo server
            client.SendAction("BROADCAST", "Hello everyone!");

            // Get the reply from our message
            var message = client.SendActionAndGetReply(actionCode, "Hello me!");
        }
    }
}
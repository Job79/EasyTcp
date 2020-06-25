using System;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    /* Examples for EasyTcp.Actions
     */
    public class ActionsExample
    {
        private const ushort Port = 6_001;

        public static void StartEchoServer()
        {
            // Create new action server, action methods are automatically loaded (See bottom of this file for action methods)
            // These examples are for the EasyTcpActionClient + EasyTcpActionServer
            var server = new EasyTcpActionServer();
            server.Start(Port);
            
            // OnUnKnownAction event, executed when an unknown action is received
            // If not used unknown actions are ignored
            server.OnUnknownAction += (sender, message) => Console.WriteLine("Unknown action received");

            /* Interceptor, function that gets called before an action is executed.
             * Action gets aborted when interceptor returns false.
             *
             * action = received message + extra property for the action code
             */
            server.Interceptor = action =>
            {
                if (action.IsAction("ECHO2")) return false;
                return true;
            };
            
            // Load more actions from other assembly
            server.AddActions(Assembly.GetExecutingAssembly(), "EasyTcp3.Examples.Actions2");
            
            // Execute action
            server.ExecuteAction("SAY_HELLO", new Message());
        }
        
        public static void Connect()
        {
            var client = new EasyTcpClient();
            if (!client.Connect("127.0.0.1", Port)) return;

            // Send action
            client.SendAction("ECHO", "Hello me!");
            client.SendAction("BROADCAST", "Hello everyone!");
            
            // Send action and get reply
            var reply = client.SendActionAndGetReply("ECHO", "Hello me!");
            Console.WriteLine(reply);

            /* All actions are converted to an int(actionCode), this is the actionCode of the "ECHO" action.
             * Integers can also be used as action codes. However this is not very readable.
             * Because everything is send as an int collisions are possible. (The algorithm used is named djb2a [http://www.cse.yorku.ca/~oz/hash.html])
             * This is no problem because this is extremely rare, however keep it in mind!
             */
            int actionCode = "ECHO".ToActionCode();
            Console.WriteLine(actionCode);
            Console.WriteLine(actionCode.IsEqualToAction("ECHO2"));
        }

        /*
         * Action methods,
         * These method are automatically triggered when receiving an action
         * See EasyTcp.Actions/ActionCore/Action for all supported delegate types (different types of methods)
         */

        /// <summary>
        /// Echo message back to the client
        /// </summary>
        /// <param name="e">received message</param>
        [EasyTcpAction("ECHO")] // Make this function an action that gets triggered when the action "ECHO" is received
        public void Echo(Message e)
            => e.Client.Send(e.Data);

        /// <summary>
        /// Broadcast received message to all connected clients
        /// </summary>
        /// <param name="sender">EasyTcpServer or EasyTcpClient as object</param>
        /// <param name="e">received message</param>
        [EasyTcpAction("BROADCAST")]
        public void Broadcast(object sender, Message e)
        {
            var server = sender as EasyTcpServer;
            server.SendAll(e.Data);
        }

        // Async actions are also supported
        [EasyTcpAction("ECHO2")]
        public async Task Ping(Message e)
            => e.Client.Send(e);

        // Actions without parameters
        [EasyTcpAction("SAY_HELLO")]
        public void DoAction() => Console.WriteLine("Hello");
    }
}

using System;
using System.Threading.Tasks;
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    /// <summary>
    /// This class contains a basic echo server made with EasyTcp3.Actions, (EasyTcpActionClient works the same!)
    /// if the action received is "ECHO" it will send the message back (like our basic echo server)
    /// if the action received is "BROADCAST" it will send the message to all connected clients
    /// </summary>
    public class ActionEchoServer
    {
        private const ushort Port = 6_001;

        public static void StartEchoServer()
        {
            // Our actions will get automatically loaded, assembly and namespace can be specified
            var server = new EasyTcpActionServer(); // Start server on port 6001
            server.Start(Port);

            // OnUnKnownAction event, executed when an unknown action is received
            server.OnUnknownAction += (sender, message) => Console.WriteLine("Unknown action received");

            // Interceptor, function that gets executed before an action is executed.
            // Action gets aborted when returning false.
            //
            // i = action code, use string.ToActionCode() for comparing with strings (See ActionClient)
            // message = receiving data
            server.Interceptor = (i, message) =>
            {
                //if(i != "ECHO".ToActionCode()) Console.WriteLine($"Received action {i}");
                return true;
            };
        }

        /*
         * Action methods,
         * These method are automatically triggered when receiving an action
         * See EasyTcp.Actions/ActionCore/Action for all supported delegate types
         */
        
        /// <summary>
        /// Echo message back to the client
        /// </summary>
        /// <param name="e">received message</param>
        [EasyTcpAction("ECHO")] // Make this function an action that will get triggered when the action "ECHO" is received
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

        /// <summary>
        /// Async actions are also supported
        /// </summary>
        [EasyTcpAction("ECHO2")]
        public async Task Ping(Message e)
            => e.Client.Send(e);

        /// <summary>
        /// Actions without parameters
        /// </summary>
        [EasyTcpAction("SAY_HELLO")]
        public void DoAction() => Console.WriteLine("Hello");
    }
}
using EasyTcp3.Actions;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    /// <summary>
    /// This class contains a basic echo server made with EasyTcp3.Actions, (there is no example for the EasyTcpAction client, because it works the same)
    /// if the action received is "ECHO" it will send the message back (like our basic echo server)
    /// if the action received is "BROADCAST" it will send the message to all connected clients
    /// </summary>
    public static class ActionEchoServer
    {
        private const ushort Port = 6_001;

        public static void StartEchoServer()
        {
            // Our actions will get automatically loaded, assembly can be specified
            var server = new EasyTcpActionServer(); //Start server on port 6001
            server.Start(Port);

            // OnUnKnownAction event, executed when an unknown action is received
            //server.OnUnknownAction += (sender, message) => { };

            // Interceptor, function that gets executed before an action is executed.
            // Action gets aborted when returning false.
            //
            // i = action code, use string.ToActionCode() for comparing with strings (See ActionClient)
            // message = receiving data
            //server.Interceptor = (i, message) => { return true; };
        }

        /// <summary>
        /// Echo message back to the client
        /// </summary>
        /// <param name="sender">EasyTcpServer as object</param>
        /// <param name="e">received message</param>
        [EasyTcpAction("ECHO")] // Make this function an action that will get triggered when the action "ECHO" is received
        public static void
            Echo(object sender,
                Message e) // Name of the function doesn't matter, but function must be static, public, void and have these two parameters
            => e.Client.Send(e.Data);

        /// <summary>
        /// Broadcast received message to all connected clients
        /// </summary>
        /// <param name="sender">EasyTcpServer as object</param>
        /// <param name="e">received message</param>
        [EasyTcpAction("BROADCAST")]
        public static void Broadcast(object sender, Message e)
        {
            var server = sender as EasyTcpServer;
            server.SendAll(e.Data);
        }
    }
}
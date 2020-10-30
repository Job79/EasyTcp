using System;
using System.Linq;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Examples.Actions
{
    /* Example authorization with EasyTcp and the IEasyTcpActionFilter interface
     */
    public class AuthorizationExample
    {
        private const ushort Port = 52512;

        public static void Start()
        {
            var server = new EasyTcpActionServer().Start(Port);
        }

        /* Example login action
         * Set userRole inside session
         */
        [EasyTcpAction("Login")]
        public void Login(Message message)
        {
            if (message.ToString() == "user")
                message.Client.Session["UserRole"] = UserRole.User;
            else if (message.ToString() == "admin")
                message.Client.Session["UserRole"] = UserRole.Admin;
            Console.WriteLine($"Authenticated {message}");
        }

        [EasyTcpAuthorization] // User does need to login before using this action
        [EasyTcpAction("Print")]
        public void UserOnlyThing(Message message) => Console.WriteLine(message);

        [EasyTcpAuthorization(UserRole.Admin)] // User does need to be admin for this action
        [EasyTcpAction("Clear")]
        public void AdminOnlyThing() => Console.Clear();
        
        [EasyTcpAuthorization]
        [EasyTcpAction("Logout")]
        public void Logout(Message message) => message.Client.Session.Remove("UserRole");
        
        public static void Connect()
        {
            using var client = new EasyTcpClient();
            if (!client.Connect("127.0.0.1", Port)) return;

            client.SendAction("Print", "This is ignored because the user is not logged in");
            client.SendAction("Login", "user");
            client.SendAction("Print", "Hello server, I am now logged in");
            client.SendAction("Clear"); // Ignored by server
            client.SendAction("Logout");
            client.SendAction("Print", "This is ignored because user is logged out");
            Console.ReadLine();
        }
    }

    /* EasyTcpAuthorization,
     * example filter attribute for authorization
     */
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EasyTcpAuthorization : EasyTcpActionFilter
    {
        private readonly UserRole[] _allowedRoles;

        /// <summary>
        /// Accept any logged in user 
        /// </summary>
        public EasyTcpAuthorization() => _allowedRoles = new[] {UserRole.User, UserRole.Admin};

        /// <summary>
        /// Accept user with a specific role
        /// </summary>
        /// <param name="role">allowed roles</param>
        public EasyTcpAuthorization(params UserRole[] role) => _allowedRoles = role;

        /// <summary>
        /// Determines whether user has access to this action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool HasAccess(object sender, Message message)
        {
            var hasRole = message.Client.Session.TryGetValue("UserRole", out object userRole);
            if (!hasRole) return false;
            
            if(_allowedRoles.Any(x => x == userRole as UserRole?)) return true;
            else return false;
        }
    }

    /* Enum with different roles */
    public enum UserRole
    {
        User,
        Admin
    }
}
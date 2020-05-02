using System.Collections.Generic;
using System.Reflection;
using EasyTcp3.Server;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// EasyTcpClient that supports 'actions'
    /// TODO
    /// </summary>
    public class EasyTcpActionServer : EasyTcpServer
    {
        internal static Dictionary<int, ActionsCore.EasyTcpActionDelegate> Actions;
        
        public EasyTcpActionServer(Assembly assembly = null, string nameSpace = null)
        {
            Actions = ActionsCore.GetActions(assembly??Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += (sender, message) => Actions.ExecuteAction(sender, message);
        }
    }
}
using System.Collections.Generic;
using System.Reflection;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// EasyTcpClient that supports 'actions'
    /// TODO
    /// </summary>
    public class EasyTcpActionClient : EasyTcpClient
    {
        internal static Dictionary<int, ActionsCore.EasyTcpActionDelegate> Actions;

        public EasyTcpActionClient(Assembly assembly = null, string nameSpace = null)
        {
            Actions = ActionsCore.GetActions(assembly??Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += (sender, message) => Actions.ExecuteAction(sender, message);
        }
    }
}
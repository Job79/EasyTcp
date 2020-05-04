using System;
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
        protected internal readonly Dictionary<int, ActionsCore.EasyTcpActionDelegate> Actions;

        public Func<int, Message, bool> Interceptor;

        public event EventHandler<Message> OnUnknownAction;

        protected internal void FireOnUnknownAction(Message e) => OnUnknownAction?.Invoke(this, e);

        public EasyTcpActionClient(Assembly assembly = null, string nameSpace = null)
        {
            Actions = ActionsCore.GetActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += (sender, message) => Actions.ExecuteAction(Interceptor,FireOnUnknownAction, sender, message);
        }
    }
}
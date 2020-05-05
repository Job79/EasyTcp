using System;
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
        protected internal readonly Dictionary<int, ActionsCore.EasyTcpActionDelegate> Actions =
            new Dictionary<int, ActionsCore.EasyTcpActionDelegate>();

        public Func<int, Message, bool> Interceptor;

        public event EventHandler<Message> OnUnknownAction;

        protected internal void FireOnUnknownAction(Message e) => OnUnknownAction?.Invoke(this, e);

        public void AddActions(Assembly assembly, string nameSpace = null)
        {
            foreach (var action in ActionsCore.GetActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace))
                Actions.Add(action.Key, action.Value);
        }

        public EasyTcpActionServer(Assembly assembly = null, string nameSpace = null)
        {
            AddActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += (sender, message) =>
                Actions.ExecuteAction(Interceptor, FireOnUnknownAction, sender, message);
        }
    }
}
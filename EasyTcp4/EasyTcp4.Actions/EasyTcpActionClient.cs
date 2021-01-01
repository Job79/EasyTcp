using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp4.Actions.ActionsCore;
using EasyTcp4.Actions.Utils;
using EasyTcp4.Protocols;

namespace EasyTcp4.Actions
{
    public class EasyTcpActionClient : EasyTcpClient
    {
        /// <summary>
        /// Dictionary with all loaded actions of client [action code, action method]
        /// </summary>
        public Dictionary<int, LoadedAction> Actions = new Dictionary<int, LoadedAction>();

        /// <summary>
        /// Function that determines whether an action should be executed
        /// </summary>
        public Func<Message, bool> Interceptor;

        /// <summary>
        /// Event that is fired when an unknown action is received 
        /// </summary>
        public event EventHandler<Message> OnUnknownAction;

        /// <summary>
        /// Fire the OnUnknownAction event
        /// </summary>
        public void FireOnUnknownAction(Message e) => OnUnknownAction?.Invoke(this, e);

        /// <summary>
        /// Load new actions from an assembly
        /// </summary>
        /// <param name="assembly">assembly with actions</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        public void LoadActionsFromAssembly(Assembly assembly, string nameSpace = null)
        {
            foreach (var action in ReflectionCore.GetActionsFromAssembly(assembly, nameSpace))
                Actions.Add(action.Key, action.Value);
        }

        /// <summary>
        /// Create new EasyTcpActionClient and load actions from (calling) assembly
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="assembly">assembly with actions, calling assembly if null</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        public EasyTcpActionClient(IEasyProtocol protocol = null, Assembly assembly = null, string nameSpace = null) : base(protocol)
        {
            LoadActionsFromAssembly(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceiveAsync += async (sender, message) =>
            {
                try { await ExecuteAction(message.ConvertToActionMessage()); }
                catch (Exception ex) { FireOnError(ex); }
            };
        }

        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="message">message with an action code</param>
        public async Task ExecuteAction(Message message)
        {
            if (Actions.TryGetValue(message.GetActionCode(), out var action)) await action.TryExecute(this, message, Interceptor);
            else OnUnknownAction?.Invoke(this, message);
        }

        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message">message without action code</param>
        public Task ExecuteAction(int actionCode, Message message = null)
            => ExecuteAction((message ?? new Message()).SetActionCode(actionCode));

        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message">message without action code</param>
        public Task ExecuteAction(string actionCode, Message message = null)
            => ExecuteAction(actionCode.ToActionCode(), message);
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp3.Actions.ActionsCore;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.Protocols;
using Action = EasyTcp3.Actions.ActionsCore.Action;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// EasyTcpClient that supports 'actions'
    /// Actions are methods with the EasyTcpAction attribute.
    /// Methods will get triggered when data is received based on a prefix(actionCode) in the received data.
    /// </summary>
    public class EasyTcpActionClient : EasyTcpClient
    {
        /// <summary>
        /// Dictionary with all loaded actions of client [action code, action method]
        /// </summary>
        protected readonly Dictionary<int, Action> Actions =
            new Dictionary<int, Action>();

        /// <summary>
        /// Function that gets called before action is executed. If function returns false discard action.
        /// </summary>
        public Func<ActionMessage, bool> Interceptor;

        /// <summary>
        /// Triggered when unknown action is received
        /// </summary>
        public event EventHandler<ActionMessage> OnUnknownAction;

        /// <summary>
        /// Fire the OnUnknownAction event
        /// </summary>
        public void FireOnUnknownAction(ActionMessage e = null) => OnUnknownAction?.Invoke(this, e);

        /// <summary>
        /// Load new actions from assembly
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">only get actions from specific namespace</param>
        public void AddActions(Assembly assembly, string nameSpace = null)
        {
            foreach (var action in ActionManager.GetActionsWithAttribute(assembly ?? Assembly.GetCallingAssembly(),
                nameSpace)) Actions.Add(action.Key, action.Value);
        }

        /// <summary>
        /// Create new EasyTcpActionClient and load actions from assembly
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="assembly">assembly with EasyTcpActions, calling assembly if null</param>
        /// <param name="nameSpace">only get actions from specific namespace</param>
        public EasyTcpActionClient(IEasyTcpProtocol protocol = null, Assembly assembly = null,
            string nameSpace = null) : base(protocol)
        {
            AddActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += async (sender, message) =>
                await Actions.ExecuteAction(Interceptor, FireOnUnknownAction, sender, message);
        }

        /// <summary>
        /// Execute specific action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ExecuteAction(int actionCode, Message message = null)
            => await Actions.ExecuteAction(Interceptor, FireOnUnknownAction, actionCode, this, message);

        /// <summary>
        /// Execute specific action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ExecuteAction(string actionCode, Message message = null)
            => await ExecuteAction(actionCode.ToActionCode(), message);

        /// <summary>
        /// Execute specific action
        /// </summary>
        /// <param name="actionMessage"></param>
        /// <returns></returns>
        public async Task ExecuteAction(ActionMessage actionMessage)
            => await ExecuteAction(actionMessage.ActionCode, actionMessage);
    }
}
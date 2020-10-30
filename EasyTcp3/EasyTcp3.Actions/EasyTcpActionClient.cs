using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp3.Actions.ActionsCore;
using EasyTcp3.Actions.ActionsCore.Reflection;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.Protocols;
using Action = EasyTcp3.Actions.ActionsCore.Action;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// EasyTcpClient with support for 'actions'
    /// Actions are methods with the EasyTcpAction attribute that get triggered when receiving a command given by the remote host.
    /// </summary>
    public class EasyTcpActionClient : EasyTcpClient
    {
        /// <summary>
        /// Dictionary with all loaded actions of client [action code, action method]
        /// </summary>
        protected readonly Dictionary<int, Action> Actions =
            new Dictionary<int, Action>();

        /// <summary>
        /// Function that determines whether action should be executed
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
        /// Load(add) new actions from an assembly
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        public void LoadActionsFromAssembly(Assembly assembly, string nameSpace = null)
        {
            foreach (var action in ReflectionCore.GetActionsFromAssembly(assembly ?? Assembly.GetCallingAssembly(),
                nameSpace)) Actions.Add(action.Key, action.Value);
        }

        /// <summary>
        /// Construct new EasyTcpClient and load actions from (calling) assembly
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="assembly">assembly with EasyTcpActions, calling assembly if null</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        public EasyTcpActionClient(IEasyTcpProtocol protocol = null, Assembly assembly = null,
            string nameSpace = null) : base(protocol)
        {
            LoadActionsFromAssembly(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceiveAsync += async (sender, message) =>
            {
                try { await message.ProcessActionMessage().HandleEasyTcpAction(sender, Actions, Interceptor, FireOnUnknownAction); }
                catch (Exception ex) { FireOnError(ex); }
            };
        }
        
        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="message">message with an action code</param>
        public async Task ExecuteAction(Message message)
            => await message.HandleEasyTcpAction(this, Actions, Interceptor, FireOnUnknownAction);
        
        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message">message without action code</param>
        public async Task ExecuteAction(int actionCode, Message message = null)
            => await (message??new Message()).SetActionCode(actionCode).HandleEasyTcpAction(this, Actions, Interceptor, FireOnUnknownAction);

        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="message">message without action code</param>
        public async Task ExecuteAction(string actionCode, Message message = null)
            => await ExecuteAction(actionCode.ToActionCode(), message);
    }
}
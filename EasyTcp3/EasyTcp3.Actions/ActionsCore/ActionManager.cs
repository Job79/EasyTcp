using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyTcp3.Actions.ActionsCore
{
    internal static class ActionManager
    {
        /// <summary>
        /// Get all methods from assembly with the EasyTcpAction attribute
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">only get actions from specific namespace</param>
        /// <returns>all EasyTcpActions within assembly</returns>
        internal static Dictionary<int, Action> GetActionsWithAttribute(Assembly assembly, string nameSpace = null)
        {
            try
            {
                var classInstances = new Dictionary<Type, object>();
                var actions = assembly.GetTypes() // Get all classes from assembly
                    // Filter namespaces when nameSpace is not null
                    .Where(t => string.IsNullOrEmpty(nameSpace) || (t.Namespace ?? "").StartsWith(nameSpace))
                    // Get methods from classes
                    .SelectMany(t => t.GetMethods())
                    // Remove non-EasyTcpAction methods 
                    .Where(Action.IsValidAction)
                    // Cast methods to Actions 
                    .ToDictionary(k => k.GetCustomAttributes().OfType<EasyTcpAction>().First().ActionCode,
                        v => new Action(v, classInstances));

                if (!actions.Any()) throw new Exception("Could not load actions: could not find any EasyTcpActions");
                return actions;
            }
            catch (MissingMethodException ex)
            {
                throw new Exception(
                    "Could not load actions: class with EasyTcpAction doesn't have a parameterless constructor", ex);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(
                    "Could not load actions: multiple methods found with the same actionCode or method does not match EasyTcpActionDelegate",
                    ex);
            }
        }

        /// <summary>
        /// Get action id from message and handle action
        /// </summary>
        /// <param name="actions">dictionary with all available actions</param>
        /// <param name="interceptor">function that gets called before action is executed. action is aborted when function returns false</param>
        /// <param name="onUnknownAction">function that gets called when action is unknown</param>
        /// <param name="sender">EasyTcpClient or EasyTcpServer as object</param>
        /// <param name="message">received data [action id : int] [data]</param>
        internal static async Task HandleAction(this Dictionary<int, Action> actions,
            Func<ActionMessage, bool> interceptor, Action<ActionMessage> onUnknownAction, object sender,
            Message message)
        {
            var actionCode = BitConverter.ToInt32(message.Data, 0); // Get action code

            // Remove action code from message
            byte[] data = null;
            if (message.Data.Length > 4)
            {
                data = new byte[message.Data.Length - 4];
                Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
            }

            await HandleAction(actions, interceptor, onUnknownAction, sender,
                new ActionMessage(data, actionCode, message.Client));
        }

        /// <summary>
        /// Handle received action
        /// </summary>
        /// <param name="actions">dictionary with all available actions</param>
        /// <param name="interceptor">function that gets called before action is executed. action is aborted when function returns false</param>
        /// <param name="onUnknownAction">function that gets called when action is unknown</param>
        /// <param name="sender">EasyTcpClient or EasyTcpServer as object</param>
        /// <param name="message">received data</param>
        internal static async Task HandleAction(this Dictionary<int, Action> actions,
            Func<ActionMessage, bool> interceptor, Action<ActionMessage> onUnknownAction, object sender,
            ActionMessage message)
        {
            if (interceptor?.Invoke(message) != false)
            {
                if (!actions.TryGetValue(message.ActionCode, out var action)) onUnknownAction?.Invoke(message); 
                else if(action.ClientHasAccess(sender, message)) await action.Execute(sender, message);
            }
        }
    }
}

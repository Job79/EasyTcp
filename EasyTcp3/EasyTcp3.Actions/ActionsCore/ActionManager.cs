using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyTcp3.Actions.ActionsCore
{
    internal static class ActionManager
    {
        /// <summary>
        /// Get all methods with the EasyTcpAction attribute
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">filter for namespace with EasyTcpActions.
        /// All actions in this namespace will be added, other will be ignored.
        /// Filter is ignored when null</param>
        /// <returns>all EasyTcpAction functions within an assembly</returns>
        /// <exception cref="Exception">could not find any EasyTcpActions</exception>
        internal static Dictionary<int, Action> GetActionsWithAttribute(Assembly assembly,
            string nameSpace = null)
        {
            try
            {
                var classInstances = new Dictionary<Type, object>();
                var actions = assembly.GetTypes() // Get all classes in assembly
                    // Filter on namespace but let everything pass if nameSpace is null
                    .Where(t => string.IsNullOrEmpty(nameSpace) || (t.Namespace ?? "").StartsWith(nameSpace))
                    // Get methods from classes
                    .SelectMany(t => t.GetMethods())
                    // Get only valid action methods
                    .Where(Action.IsValidAction)
                    // Cast methods to dictionary
                    .ToDictionary(k => k.GetCustomAttributes().OfType<EasyTcpAction>().First().ActionCode,
                        v => new Action(v, classInstances));

                if (!actions.Any()) throw new Exception("Could not find any EasyTcpActions");
                return actions;
                // TODO, does classInstances gets disposed? Does it still work when disposed?
            }
            catch (ArgumentException ex)
            {
                throw new Exception(
                    "Could not load actions: multiple methods found with the same actionCode or method does not match EasyTcpActionDelegate",
                    ex);
            }
        }

        /// <summary>
        /// Execute a received action
        /// </summary>
        /// <param name="actions">dictionary with all available actions</param>
        /// <param name="interceptor">Function that gets called before action is executed.
        /// If function returns false discard action. Ignore parameter when null</param>
        /// <param name="onUnknownAction">action that gets triggered when an unknown action is received</param>
        /// <param name="sender">EasyTcpClient or EasyTcpServer as object</param>
        /// <param name="message">received data [action id : int] [data]</param>
        internal static void ExecuteAction(this Dictionary<int, Action> actions,
            Func<int, Message, bool> interceptor, Action<Message> onUnknownAction, object sender,
            Message message)
        {
            var actionCode = BitConverter.ToInt32(message.Data, 0); // Get action code as int
             
            if (!actions.TryGetValue(actionCode, out var action))
            {
                onUnknownAction?.Invoke(message);
                return;
            }

            // Remove action code from message
            byte[] data = null;
            if (message.Data.Length > 4)
            {
#if !NETSTANDARD2_1
                data = message.Data[4..];
#else
                data = new byte[message.Data.Length - 4];
                Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
#endif
            }

            // Execute action
            var m = new Message(data, message.Client);
            if (interceptor?.Invoke(actionCode, m) != false) action.Execute(sender, m);
        }
    }
}
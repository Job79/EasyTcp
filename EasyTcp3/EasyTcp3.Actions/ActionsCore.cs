using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Contains the core elements of the action system
    ///
    /// Protocol of the action system:
    /// (ushort: [data length + action id length(4)]) [int: action id] [data]
    /// </summary>
    public static class ActionsCore
    {
        /// <summary>
        /// Delegate of an EasyTcp action
        /// See it as an template
        /// </summary>
        /// <param name="sender">EasyTcpClient or EasyTcpServer as object</param>
        /// <param name="message">received message</param>
        public delegate void EasyTcpActionDelegate(object sender, Message message);

        /// <summary>
        /// Get all methods with the EasyTcpAction attribute
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">filter for namespace with EasyTcpActions.
        /// All actions in this namespace will be added, other will be ignored.
        /// Filter is ignored when null</param>
        /// <returns>all EasyTcpAction functions within an assembly</returns>
        /// <exception cref="Exception">could not find any EasyTcpActions</exception>
        internal static Dictionary<int, EasyTcpActionDelegate> GetActions(Assembly assembly,
            string nameSpace = null)
        {
            try
            {
                var actions = assembly.GetTypes() // Get all classes in assembly
                    // Filter on namespace but pass everything if nameSpace is null
                    .Where(t => string.IsNullOrEmpty(nameSpace) || (t.Namespace ?? "").StartsWith(nameSpace))
                    // Get methods from classes
                    .SelectMany(t => t.GetMethods())
                    // Get valid action methods
                    .Where(IsValidMethod)
                    // Cast methods to dictionary
                    .ToDictionary(k => k.GetCustomAttributes().OfType<EasyTcpAction>().First().ActionCode,
                        v => (EasyTcpActionDelegate) Delegate.CreateDelegate(typeof(EasyTcpActionDelegate), v));

                if (!actions.Any()) throw new Exception("Could not find any EasyTcpActions");
                return actions;
            }
            catch (ArgumentException ex)
            {
                throw new Exception(
                    "Could not load actions: multiple methods found with the same actionCode or method does not match EasyTcpActionDelegate",
                    ex);
            }
        }

        /// <summary>
        /// Determines whether a method is a valid action method
        /// </summary>
        /// <param name="m">method</param>
        /// <returns>true if method is valid</returns>
        private static bool IsValidMethod(MethodInfo m)
        {
            if (!m.GetCustomAttributes().OfType<EasyTcpAction>().Any() || !m.IsStatic) return false;

            var parameters = m.GetParameters();
            if (parameters.Length != 2 || parameters[0].ParameterType != typeof(object) ||
                parameters[1].ParameterType != typeof(Message)) return false;
            return true;
        }

        /// <summary>
        /// Execute a received action
        /// </summary>
        /// <param name="actions">dictionary with all available actions</param>
        /// <param name="interceptor">Function that gets called before action is executed.
        /// If function returns false discard action. Ignore parameter when null</param>
        /// <param name="onUnknownAction">action that gets triggered when an unknown action is received</param>
        /// <param name="sender">EasyTcpClient or EasyTcpServer as object</param>
        /// <param name="message">received data [int: action id] [data]</param>
        internal static void ExecuteAction(this Dictionary<int, EasyTcpActionDelegate> actions,
            Func<int, Message, bool> interceptor, Action<Message> onUnknownAction, object sender,
            Message message)
        {
            var actionCode = BitConverter.ToInt32(message.Data, 0);
            actions.TryGetValue(actionCode, out var action);
            if (action == null)
            {
                onUnknownAction?.Invoke(message);
                return;
            }

#if !NETSTANDARD2_1
            var data = message.Data[4..];
#else
            var data = new byte[message.Data.Length - 4];
            Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
#endif
            var m = new Message(data, message.Client);
            if (interceptor?.Invoke(actionCode, m) != false) action.Invoke(sender, m);
        }

        /// <summary>
        /// Convert a string to an actionCode by using the djb2a hashing algorithm
        /// ! Collision are surely possible, but this shouldn't be a problem here (for example: haggadot & loathsomenesses)
        /// http://www.cse.yorku.ca/~oz/hash.html
        /// </summary>
        /// <param name="str">action id as string</param>
        /// <returns>action id as int</returns>
        public static int ToActionCode(this string str)
        {
            int hash = 5381;
            foreach (var t in str) hash = ((hash << 5) + hash) ^ (byte) t;
            return hash;
        }
    }
}
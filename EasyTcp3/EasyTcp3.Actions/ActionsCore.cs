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
    /// ([data length + action id length(4)]) [action id (int)] [data]
    /// </summary>
    public static class ActionsCore
    {
        public delegate void EasyTcpActionDelegate(object sender, Message message);

        /// <summary>
        /// Get all methods with the EasyTcpAction attribute
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpAction</param>
        /// <param name="nameSpace">namespace with EasyTcpActions, filter is ignored when null</param>
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
                throw new Exception("Could not load actions: multiple methods found with the same actionCode", ex);
            }
        }

        /// <summary>
        /// Determines whether a method is a valid action method
        /// </summary>
        /// <param name="m">method</param>
        /// <returns></returns>
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
        /// <param name="actions"></param>
        /// <param name="interceptor"></param>
        /// <param name="onUnknownAction"></param>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        internal static void ExecuteAction(this Dictionary<int, EasyTcpActionDelegate> actions,
            Func<int, Message, bool> interceptor, Action onUnknownAction, object sender,
            Message message)
        {
            var actionCode = BitConverter.ToInt32(message.Data, 0);
            actions.TryGetValue(actionCode, out var action);
            if (action == null)
            {
                onUnknownAction?.Invoke();
                return;
            }

#if !NETSTANDARD2_1
            action.Invoke(sender, new Message(message.Data[4..], message.Client));
#else
            var data = new byte[message.Data.Length - 4];
            Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
#endif
            var m = new Message(data, message.Client);
            if (interceptor?.Invoke(actionCode, m) != false) action.Invoke(sender, m);
        }

        /// <summary>
        /// Convert a string to an ActionCode by using the djb2a hashing algorithm
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
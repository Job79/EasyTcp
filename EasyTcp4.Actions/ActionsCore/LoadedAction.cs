using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp4.Actions.Utils;

namespace EasyTcp4.Actions.ActionsCore
{
    public class LoadedAction
    {
        /// <summary>
        /// Valid action delegate
        /// </summary>
        private readonly Delegate ActionMethod;

        /// <summary>
        /// Array with action filters
        /// </summary>
        public readonly EasyActionFilter[] Filters;

        /// <summary>
        /// Create new action
        /// </summary>
        /// <param name="actionMethod">valid action method</param>
        /// <param name="classInstance">instance of the declaring class of the actionMethod, null when actionMethod is static</param>
        public LoadedAction(MethodInfo actionMethod, object classInstance)
        {
            var methodType = actionMethod.GetActionDelegateType();
            ActionMethod = Delegate.CreateDelegate(methodType, classInstance, actionMethod);
            Filters = actionMethod.GetCustomAttributes().OfType<EasyActionFilter>().ToArray();
        }

        /// <summary>
        /// Trigger actionMethod
        /// </summary>
        /// <param name="sender">instance of an EasyTcpClient or EasyTcpServer</param>
        /// <param name="message">received message</param>
        /// <param name="interceptor">function that determines whether action should be executed</param>
        public async Task TryExecute(object sender = null, Message message = null, Func<Message, bool> interceptor = null)
        {
            if (interceptor?.Invoke(message) != false && HasAccess(sender, message))
                await Delegates.ExecuteActionDelegate(ActionMethod, sender, message);
        }

        /// <summary>
        /// Determines whether the remote host has access to this action based on the filter attributes
        /// </summary>
        private bool HasAccess(object sender, Message message)
        {
            if (Filters == null) return true;
            foreach (var filter in Filters)
                if (!filter.HasAccess(sender, message))
                    return false;
            return true;
        }
    }
}

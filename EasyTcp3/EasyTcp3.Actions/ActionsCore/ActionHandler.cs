using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions.ActionsCore
{
    public static class ActionHandler
    {
        /// <summary>
        /// Execute action when allowed by the interceptor and filter attributes
        /// </summary>
        /// <param name="message">attribute of action method and contains the action code</param>
        /// <param name="sender">attribute of action method</param>
        /// <param name="actions">list with available actions</param>
        /// <param name="interceptor">interceptor delegate</param>
        /// <param name="onUnknownAction">onUnknownAction delegate</param>
        /// <returns></returns>
        internal static async Task HandleEasyTcpAction(this Message message, object sender, Dictionary<int, Action> actions,
            Func<Message, bool> interceptor, Action<Message> onUnknownAction)
        {
            if (interceptor?.Invoke(message) != false)
            {
                if (!actions.TryGetValue(message.GetActionCode(), out var action)) onUnknownAction?.Invoke(message); 
                else if(action.HasAccessTo(sender, message)) await action.Execute(sender, message);
            }
        }
    }
}
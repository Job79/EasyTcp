using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp3.Actions.ActionsCore.Reflection;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions.ActionsCore
{
    public class Action
    {
        /// <summary>
        /// Instance of an EasyTcpActionDelegate
        /// </summary>
        protected readonly Delegate ActionMethod;
        
        /// <summary>
        /// List with EasyTcpAction filters
        /// </summary>
        public readonly List<EasyTcpActionFilter> Filters;

        /// <summary>
        /// Construct a new action
        /// </summary>
        /// <param name="actionMethod">method that matches an EasyTcpActionDelegate</param>
        /// <param name="classInstance">instance of the declaring class of the actionMethod, null when actionMethod is static</param>
        public Action(MethodInfo actionMethod, object classInstance)
        {
            var methodType = actionMethod.GetActionDelegateType();
            ActionMethod = Delegate.CreateDelegate(methodType, classInstance, actionMethod);
            Filters = actionMethod.GetCustomAttributes().OfType<EasyTcpActionFilter>().ToList();
        }
        
        /// <summary>
        /// Determines whether the remote host has access to this action based on filter attributes
        /// </summary>
        /// <param name="sender">instance of an EasyTcpClient or EasyTcpServer</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HasAccessTo(object sender, Message message)
        {
            if (Filters == null) return true;
            foreach (var filter in Filters)
                if (!filter.HasAccess(sender, message))
                    return false;
            return true;
        }
        
        /// <summary>
        /// Trigger actionMethod
        /// </summary>
        /// <param name="sender">instance of an EasyTcpClient or EasyTcpServer</param>
        /// <param name="message">received message</param>
        public async Task Execute(object sender = null, Message message = null)
        {
            var type = ActionMethod.GetType();
            if (type == typeof(Delegates.ActionDelegate))
                ((Delegates.ActionDelegate) ActionMethod)(sender, message);
            else if (type == typeof(Delegates.ActionDelegate1))
                ((Delegates.ActionDelegate1) ActionMethod)(message);
            else if (type == typeof(Delegates.ActionDelegate2))
                ((Delegates.ActionDelegate2) ActionMethod)();
            else if (type == typeof(Delegates.ActionDelegate3))
                await ((Delegates.ActionDelegate3) ActionMethod)(sender, message);
            else if (type == typeof(Delegates.ActionDelegate4))
                await ((Delegates.ActionDelegate4) ActionMethod)(message);
            else if (type == typeof(Delegates.ActionDelegate5))
                await ((Delegates.ActionDelegate5) ActionMethod)();
        }
    }
}
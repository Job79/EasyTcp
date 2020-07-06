using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions.ActionsCore
{
    /// <summary>
    /// Class representing an EasyTcpAction method 
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Different EasyTcpAction delegate types
        /// </summary>
        private delegate void EasyTcpActionDelegate(object sender, Message message);

        private delegate void EasyTcpActionDelegate1(Message message);

        private delegate void EasyTcpActionDelegate2();

        private delegate Task EasyTcpActionDelegate3(object sender, Message message);

        private delegate Task EasyTcpActionDelegate4(Message message);

        private delegate Task EasyTcpActionDelegate5();

        /// <summary>
        /// List with EasyTcpAction filters
        /// </summary>
        public List<EasyTcpActionFilter> Filters;

        /// <summary>
        /// Instance of EasyTcpActionDelegate*
        /// </summary>
        private Delegate EasyTcpAction;

        /// <summary>
        /// Create new action
        /// </summary>
        /// <param name="method">method that matches an EasyTcpActionDelegate</param>
        /// <param name="classInstances">list with initialized classes</param>
        public Action(MethodInfo method, Dictionary<Type, object> classInstances)
        {
            var classInstance = GetClassInstance(method, classInstances);
            var methodType = GetDelegateType(method);

            EasyTcpAction = Delegate.CreateDelegate(methodType, classInstance, method);

            var filters = method.GetCustomAttributes().OfType<EasyTcpActionFilter>().ToList();
            if (filters.Any()) Filters = filters;
        }
        
        /// <summary>
        /// Get instance of declaring class
        /// get instance from classInstances when possible,
        /// else create a new instance
        /// </summary>
        /// <param name="method">method that matches a EasyTcpActionDelegate</param>
        /// <param name="classInstances">list with initialized classes</param>
        /// <returns>null if method is static, else instance of declaring class</returns>
        private static object GetClassInstance(MethodInfo method, Dictionary<Type, object> classInstances)
        {
            if (method.IsStatic) return null;

            var classType = method.DeclaringType;
            if (!classInstances.TryGetValue(classType ?? throw new InvalidOperationException("Declaring class is null"), out object instance))
            {
                instance = Activator.CreateInstance(classType);
                classInstances.Add(classType, instance);
            }

            return instance;
        }

        /// <summary>
        /// Determines whether client has access to a action based on filter attributes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ClientHasAccess(object sender, ActionMessage message)
        {
            if (Filters == null) return true;
            foreach (var filter in Filters)
                if (!filter.HasAccess(sender, message))
                    return false;
            return true;
        }
        
        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="sender">instance of EasyTcpClient or EasyTcpServer</param>
        /// <param name="message">received message</param>
        public async Task Execute(object sender = null, Message message = null)
        {
            var type = EasyTcpAction.GetType();
            if (type == typeof(EasyTcpActionDelegate))
                ((EasyTcpActionDelegate) EasyTcpAction)(sender, message);
            else if (type == typeof(EasyTcpActionDelegate1))
                ((EasyTcpActionDelegate1) EasyTcpAction)(message);
            else if (type == typeof(EasyTcpActionDelegate2))
                ((EasyTcpActionDelegate2) EasyTcpAction)();
            else if (type == typeof(EasyTcpActionDelegate3))
                await ((EasyTcpActionDelegate3) EasyTcpAction)(sender, message);
            else if (type == typeof(EasyTcpActionDelegate4))
                await ((EasyTcpActionDelegate4) EasyTcpAction)(message);
            else if (type == typeof(EasyTcpActionDelegate5))
                await ((EasyTcpActionDelegate5) EasyTcpAction)();
        }

        /// <summary>
        /// Get EasyTcpActionDelegate type from methodInfo 
        /// </summary>
        /// <param name="m"></param>
        /// <returns>type of EasyTcpActionDelegate or null when none</returns>
        private static Type GetDelegateType(MethodInfo m)
        {
            if (m.ReturnType == typeof(void))
            {
                var p = m.GetParameters();
                if (p.Length == 2 && p[0].ParameterType == typeof(object) && p[1].ParameterType == typeof(Message))
                    return typeof(EasyTcpActionDelegate);
                if (p.Length == 1 && p[0].ParameterType == typeof(Message))
                    return typeof(EasyTcpActionDelegate1);
                if (p.Length == 0 && m.ReturnType == typeof(void)) return typeof(EasyTcpActionDelegate2);
            }
            else if (m.ReturnType == typeof(Task))
            {
                var p = m.GetParameters();
                if (p.Length == 2 && p[0].ParameterType == typeof(object) && p[1].ParameterType == typeof(Message))
                    return typeof(EasyTcpActionDelegate3);
                if (p.Length == 1 && p[0].ParameterType == typeof(Message))
                    return typeof(EasyTcpActionDelegate4);
                if (p.Length == 0) return typeof(EasyTcpActionDelegate5);
            }
            return null;
        }

        /// <summary>
        /// Determines whether method is a valid EasyTcpAction
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if method is a valid action</returns>
        public static bool IsValidAction(MethodInfo m) =>
            m.GetCustomAttributes(typeof(EasyTcpAction), false).Any() && GetDelegateType(m) != null;
    }
}
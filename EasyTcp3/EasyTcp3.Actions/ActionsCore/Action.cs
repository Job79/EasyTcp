using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyTcp3.Actions.ActionsCore
{
    public class Action
    {
        /// <summary>
        /// Different EasyTcpAction delegate types
        /// </summary>
        private delegate void EasyTcpActionDelegate(object sender, Message message);

        private delegate void EasyTcpActionDelegate1(Message message);

        private delegate void EasyTcpActionDelegate2();

        /// <summary>
        /// Instance of any EasyTcpActionDelegate
        /// </summary>
        private Delegate EasyTcpAction;

        /// <summary>
        /// Create new action
        /// </summary>
        /// <param name="method">method that matches any EasyTcpActionDelegate</param>
        /// <param name="classInstances">dictionary with instances of already initialized classes</param>
        public Action(MethodInfo method, Dictionary<Type, object> classInstances)
        {
            var classInstance = GetClassInstance(method, classInstances);
            var methodType = GetDelegateType(method);

            if (classInstance == null) EasyTcpAction = Delegate.CreateDelegate(methodType, method);
            EasyTcpAction = Delegate.CreateDelegate(methodType, classInstance, method);
        }

        /// <summary>
        /// Executes action
        /// </summary>
        /// <param name="sender">instance of EasyTcpClient or EasyTcpServer</param>
        /// <param name="message">received message</param>
        public void Execute(object sender = null, Message message = null)
        {
            var type = EasyTcpAction.GetType();
            if (type == typeof(EasyTcpActionDelegate))
                ((EasyTcpActionDelegate) EasyTcpAction)(sender, message);
            else if (type == typeof(EasyTcpActionDelegate1))
                ((EasyTcpActionDelegate1) EasyTcpAction)(message);
            else ((EasyTcpActionDelegate2) EasyTcpAction)();
        }

        /// <summary>
        /// Get instance of declaring class
        /// gets instance from classInstances when possible,
        /// else add new instance to classInstances
        /// </summary>
        /// <param name="method">method that matches any EasyTcpActionDelegate</param>
        /// <param name="classInstances">list with initialized classes</param>
        /// <returns>null if method is static, else instance of declaring class</returns>
        private static object GetClassInstance(MethodInfo method, Dictionary<Type, object> classInstances)
        {
            if (method.IsStatic) return null;

            var classType = method.DeclaringType;
            if (!classInstances.TryGetValue(classType, out object instance))
            {
                instance = Activator.CreateInstance(classType);
                classInstances.Add(classType, instance);
            }

            return instance;
        }

        /// <summary>
        /// Get EasyTcpActionDelegate type from methodInfo 
        /// </summary>
        /// <param name="m"></param>
        /// <returns>type of any EasyTcpActionDelegate or null when none</returns>
        private static Type GetDelegateType(MethodInfo m)
        {
            var p = m.GetParameters();

            if (p.Length == 2 && m.ReturnType == typeof(void) && p[0].ParameterType == typeof(object) &&
                p[1].ParameterType == typeof(Message))
                return typeof(EasyTcpActionDelegate);
            else if (p.Length == 1 && m.ReturnType == typeof(void) && p[0].ParameterType == typeof(Message)) return typeof(EasyTcpActionDelegate1);
            else if (p.Length == 0 && m.ReturnType == typeof(void)) return typeof(EasyTcpActionDelegate2);
            else return null;
        }

        /// <summary>
        /// Determines whether a method is a valid action
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if method is a valid action</returns>
        public static bool IsValidAction(MethodInfo m) =>
            m.GetCustomAttributes().OfType<EasyTcpAction>().Any() && GetDelegateType(m) != null;
    }
}
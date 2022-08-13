using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyTcp4.Actions.ActionsCore
{
    public static class Delegates
    {
        /* All accepted delegate types */
        private delegate void Delegate1(object sender, Message message);
        private delegate void Delegate2(Message message);
        private delegate void Delegate3();
        private delegate Task Delegate4(object sender, Message message);
        private delegate Task Delegate5(Message message);
        private delegate Task Delegate6();

        /// <summary>
        /// Get delegate type for a specific action method
        /// Return null when method isn't a valid action method.
        /// </summary>
        internal static Type GetActionDelegateType(this MethodInfo m)
        {
            bool isVoid = m.ReturnType == typeof(void), isTask = m.ReturnType == typeof(Task);
            if (!isVoid && !isTask) return null;

            var p = m.GetParameters();
            if (p.Length == 1 && p[0].ParameterType == typeof(Message))
                return isVoid ? typeof(Delegate2) : typeof(Delegate5);
            if (p.Length == 2 && p[0].ParameterType == typeof(object) && p[1].ParameterType == typeof(Message))
                return isVoid ? typeof(Delegate1) : typeof(Delegate4);
            if (p.Length == 0)
                return isVoid ? typeof(Delegate3) : typeof(Delegate6);
            return null;
        }

        /// <summary>
        /// Execute an action mehod
        /// </summary>
        /// <param name="d">valid action delegate</param>
        /// <param name="sender">instance of an EasyTcpClient or EasyTcpServer</param>
        /// <param name="message">received message</param>
        internal static async Task ExecuteActionDelegate(Delegate d, object sender, Message message)
        {
            var type = d.GetType();
            if (type == typeof(Delegate1)) ((Delegate1)d)(sender, message);
            else if (type == typeof(Delegate2)) ((Delegate2)d)(message);
            else if (type == typeof(Delegate3)) ((Delegate3)d)();
            else if (type == typeof(Delegate4)) await ((Delegate4)d)(sender, message);
            else if (type == typeof(Delegate5)) await ((Delegate5)d)(message);
            else if (type == typeof(Delegate6)) await ((Delegate6)d)();
        }
    }
}

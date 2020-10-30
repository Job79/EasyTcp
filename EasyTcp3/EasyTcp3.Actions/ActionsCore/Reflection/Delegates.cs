using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyTcp3.Actions.ActionsCore.Reflection
{
    public static class Delegates
    {
        /* All accepted delegate types */
        internal delegate void ActionDelegate(object sender, Message message);
        internal delegate void ActionDelegate1(Message message);
        internal delegate void ActionDelegate2();
        internal delegate Task ActionDelegate3(object sender, Message message);
        internal delegate Task ActionDelegate4(Message message);
        internal delegate Task ActionDelegate5();

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
                return isVoid ? typeof(ActionDelegate1) : typeof(ActionDelegate4);
            if (p.Length == 2 && p[0].ParameterType == typeof(object) && p[1].ParameterType == typeof(Message))
                return isVoid ? typeof(ActionDelegate) : typeof(ActionDelegate3);
            if (p.Length == 0)
                return isVoid ? typeof(ActionDelegate2) : typeof(ActionDelegate5);
            return null;
        }
    }
}
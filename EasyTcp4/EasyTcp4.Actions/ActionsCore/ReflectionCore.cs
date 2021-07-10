using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyTcp4.Actions.ActionsCore
{
    internal static class ReflectionCore
    {
        /// <summary>
        /// Get all methods with the action attribute from an assembly
        /// </summary>
        /// <param name="assembly">assembly with actions</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        /// <returns>all valid actions within an assembly</returns>
        internal static Dictionary<int, LoadedAction> GetActionsFromAssembly(Assembly assembly, string nameSpace = null)
        {
            try
            {
                var classInstances = new Dictionary<Type, object>();
                return assembly.GetTypes()
                    .Where(t => string.IsNullOrEmpty(nameSpace) || (t.Namespace ?? "").StartsWith(nameSpace))
                    .SelectMany(t => t.GetMethods())
                    .Where(IsValidAction)
                    .ToDictionary(k => k.GetCustomAttributes().OfType<EasyAction>().First().ActionCode,
                        v => new LoadedAction(v, GetClassInstance(v, classInstances)));
            }
            catch (MissingMethodException ex)
            {
                throw new Exception("Could not load actions, make sure that the classes with (non-static) actions do hava a parameterless constructor\n", ex);
            }
            catch (ArgumentException ex)
            {
                throw new Exception("Could not load actions, make sure that there aren't multiple actions with the same action code\n", ex);
            }
        }

        /// <summary>
        /// Determines whether method is a valid action
        /// </summary>
        private static bool IsValidAction(this MethodInfo m) =>
            m.GetCustomAttributes(typeof(EasyAction), false).Any() && m.GetActionDelegateType() != null;

        /// <summary>
        /// Get a class instance for an action
        /// New class instance is created when not available inside classInstances.
        /// </summary>
        /// <param name="method">valid action method</param>
        /// <param name="classInstances">list with already initialized classes</param>
        /// <returns>null if method is static, else instance of declaring class</returns>
        private static object GetClassInstance(MethodInfo method, Dictionary<Type, object> classInstances)
        {
            if (method.IsStatic) return null; // Static actions don't need a class instance

            var classType = method.DeclaringType ?? throw new InvalidOperationException("Declaring class is null");
            if (!classInstances.TryGetValue(classType, out object instance))
            {
                instance = Activator.CreateInstance(classType);
                classInstances.Add(classType, instance);
            }

            return instance;
        }
    }
}

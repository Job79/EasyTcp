using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyTcp3.Actions.ActionsCore.Reflection
{
    public static class ReflectionCore
    {
        /// <summary>
        /// Get all methods with the EasyTcpAction attribute from an assembly
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">only load actions from a specific namespace</param>
        /// <returns>all valid EasyTcpActions within the assembly</returns>
        internal static Dictionary<int, Action> GetActionsFromAssembly(Assembly assembly, string nameSpace = null)
        {
            try
            {
                var classInstances = new Dictionary<Type, object>();
                return assembly.GetTypes() 
                    .Where(t => string.IsNullOrEmpty(nameSpace) || (t.Namespace ?? "").StartsWith(nameSpace))
                    .SelectMany(t => t.GetMethods()) 
                    .Where(IsValidEasyTcpAction)
                    .ToDictionary(k => k.GetCustomAttributes().OfType<EasyTcpAction>().First().ActionCode,
                        v => new Action(v, GetClassInstance(v, classInstances))); 
            }
            catch (MissingMethodException ex)
            {
                throw new Exception(
                    "Could not load actions, see inner exception for more details\nMake sure that the classes with EasyTcpActions do have a parameterless constructor", ex);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(
                    "Could not load actions, see inner exception for more details\nMake sure that there aren't multiple actions with the same action code",
                    ex);
            }
        }
        
        /// <summary>
        /// Determines whether method is a valid EasyTcpAction
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if method is a valid action</returns>
        private static bool IsValidEasyTcpAction(this MethodInfo m) =>
            m.GetCustomAttributes(typeof(EasyTcpAction), false).Any() && m.GetActionDelegateType() != null;
        
        /// <summary>
        /// Get a class instance for non-static methods
        /// New class instance is created when not available inside classInstances.
        /// </summary>
        /// <param name="method">valid EasyTcpActionMethod</param>
        /// <param name="classInstances">list with already initialized classes</param>
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
    }
}
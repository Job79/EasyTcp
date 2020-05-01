using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyTcp3.Server;

namespace EasyTcp3.Actions
{
    public static class ActionUtil
    {
        private static Dictionary<int, EasyTcpActionDelegate> _actions;

        private delegate void EasyTcpActionDelegate(object sender, Message message);

        public static void StartActionsUtil(this EasyTcpServer server, Assembly assembly = null,
            string nameSpace = null)
        {
                _actions = (assembly ?? Assembly.GetCallingAssembly()).GetTypes()
                .Where(t => string.IsNullOrEmpty(nameSpace) ||
                            (t.Namespace ?? "")
                            .StartsWith(nameSpace)) // Filter on namespace but pass everything if nameSpace is null
                .SelectMany(t => t.GetMethods())
                .Where(IsValidMethod)
                .ToDictionary(k => k.GetCustomAttributes().OfType<EasyTcpAction>().First().OperationCode,
                    v => (EasyTcpActionDelegate)Delegate.CreateDelegate(typeof(EasyTcpActionDelegate), v));
            
            if(!_actions.Any()) throw new Exception("Could not find any EasyTcpActions");

            server.OnDataReceive += HandleReceivedData;
        }

        private static void HandleReceivedData(object sender, Message e)
        {
            int operationCode = BitConverter.ToInt32(e.Data,0);
            _actions.TryGetValue(operationCode, out var operation);
            if(operation == null) return;
#if !NETSTANDARD2_1
            var data = e.Data[4..];
#else 
            var data = new byte[e.Data.Length-4];
            Buffer.BlockCopy(e.Data,4,data,0,data.Length);
#endif
            operation.Invoke(sender,new Message(data, e.Client));
        }

        private static bool IsValidMethod(MethodInfo m)
        {
            if (!m.GetCustomAttributes().OfType<EasyTcpAction>().Any() || !m.IsStatic) return false;

            var parameters = m.GetParameters();
            if (parameters.Length != 2 || parameters[0].ParameterType != typeof(object) ||
                parameters[1].ParameterType != typeof(Message)) return false; 
            return true;
        }
    }
}
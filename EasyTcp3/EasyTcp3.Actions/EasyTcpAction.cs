using System;

namespace EasyTcp3.Actions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EasyTcpAction : Attribute
    {
        public int OperationCode { get; private set; }

        public EasyTcpAction(int operationCode)
        {
            OperationCode = operationCode;
        }
    }
}
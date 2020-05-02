using System;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Attribute type for EasyTcpActions, only methods with this attribute will get loaded as actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EasyTcpAction : Attribute
    {
        /// <summary>
        /// Code of this action,
        /// error will be thrown if not unique
        /// </summary>
        public int ActionCode { get; set; }

        public EasyTcpAction(int actionCode)  => ActionCode = actionCode;
        public EasyTcpAction(string actionCode) => ActionCode = actionCode.ToActionCode();
    }
}
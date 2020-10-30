using System;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Attribute for EasyTcpActions, methods with this attribute will get detected as actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EasyTcpAction : Attribute
    {
        /// <summary>
        /// ActionCode of action, used to determine action method when receiving data
        /// Error will be thrown if not unique
        /// </summary>
        public int ActionCode { get; }

        /// <summary></summary>
        /// <param name="actionCode">action code</param>
        public EasyTcpAction(int actionCode) => ActionCode = actionCode;

        /// <summary>
        /// Create action with a string as actionCode, string gets converted to an int with djb2a
        /// </summary>
        /// <param name="actionCode">action code as string</param>
        public EasyTcpAction(string actionCode) => ActionCode = actionCode.ToActionCode();
    }
}
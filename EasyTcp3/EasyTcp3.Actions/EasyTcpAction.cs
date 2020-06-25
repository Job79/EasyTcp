using System;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Attribute for EasyTcpActions, methods with this attribute will get loaded as actions
    /// See examples for usage
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EasyTcpAction : Attribute
    {
        /// <summary>
        /// Code of this action, used to determine action method when receiving data
        /// error will be thrown if not unique
        /// </summary>
        public int ActionCode { get; }

        /// <summary>
        /// Create action with specific action code
        /// </summary>
        /// <param name="actionCode">action code</param>
        public EasyTcpAction(int actionCode) => ActionCode = actionCode;

        /// <summary>
        /// Create action with string converted to action code
        /// </summary>
        /// <param name="actionCode">action code as string</param>
        public EasyTcpAction(string actionCode) => ActionCode = actionCode.ToActionCode();
    }
}
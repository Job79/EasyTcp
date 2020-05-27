using System;
using EasyTcp3.Actions.ActionUtils;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Attribute type for EasyTcpActions, only methods with this attribute will get loaded as actions
    /// See the examples for usage
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] // This attribute is only valid for methods ((static) Functions to be specific)
    public class EasyTcpAction : Attribute
    {
        /// <summary>
        /// Code of this action,
        /// error will be thrown if not unique
        /// </summary>
        public int ActionCode { get; set; }

        /// <summary>
        /// Create action with a specific action code
        /// </summary>
        /// <param name="actionCode">action code</param>
        public EasyTcpAction(int actionCode) => ActionCode = actionCode;

        /// <summary>
        /// Create action with a converted string as action code
        /// </summary>
        /// <param name="actionCode">action code as string</param>
        public EasyTcpAction(string actionCode) => ActionCode = actionCode.ToActionCode();
    }
}
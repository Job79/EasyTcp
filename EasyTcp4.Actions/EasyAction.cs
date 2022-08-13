using System;
using EasyTcp4.Actions.Utils;

namespace EasyTcp4.Actions
{
    /// <summary>
    /// Attribute for the actions, methods with this attribute will get detected as actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EasyAction : Attribute
    {
        /// <summary>
        /// ActionCode of action, used to determine action method when receiving data
        /// ActionCode must be unique
        /// </summary>
        public int ActionCode { get; }

        /// <summary></summary>
        /// <param name="actionCode">action code</param>
        public EasyAction(int actionCode) => ActionCode = actionCode;

        /// <summary>
        /// Create action with a string as actionCode, string gets converted to an int with djb2a
        /// </summary>
        /// <param name="actionCode">action code as string</param>
        public EasyAction(string actionCode) => ActionCode = actionCode.ToActionCode();
    }
}

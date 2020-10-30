using System;

namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Filter attribute for EasyTcpActions, used to implement authorization
    /// </summary>
    public abstract class EasyTcpActionFilter : Attribute
    {
        /// <summary>
        /// Determines whether client has access to an action
        /// Action call is aborted when function returns false.
        /// </summary>
        /// <param name="sender">EasyTcpActionServer/EasyTcpActionClient as sender</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract bool HasAccess(object sender, Message message);
    }
}
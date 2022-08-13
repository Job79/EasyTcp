using System;

namespace EasyTcp4.Actions.Utils
{
    public abstract class EasyActionFilter : Attribute
    {
        /// <summary>
        /// Determines whether client has access to an action
        /// Action call is aborted when function returns false.
        /// </summary>
        /// <param name="sender">EasyTcpActionServer/EasyTcpActionClient as sender</param>
        /// <param name="message">received action message</param>
        public abstract bool HasAccess(object sender, Message message);
    }
}

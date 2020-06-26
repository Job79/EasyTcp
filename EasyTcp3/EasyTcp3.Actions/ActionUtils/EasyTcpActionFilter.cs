namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Filter attribute for EasyTcpActions 
    /// </summary>
    public interface IEasyTcpActionFilter
    {
        /// <summary>
        /// Determines whether client has access to an action
        /// action is aborted when function returns false
        /// </summary>
        /// <param name="sender">EasyTcpActionServer/EasyTcpActionClient as sender</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HasAccess(object sender, ActionMessage message);
    }
}
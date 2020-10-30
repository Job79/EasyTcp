using System;

namespace EasyTcp3.Actions.ActionUtils
{
    public static class ActionMessageUtil
    {
        /// <summary>
        /// Store the value of an action code inside a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="actionCode">new value</param>
        public static Message SetActionCode(this Message message, int actionCode)
        {
            message.MetaData["ActionCode"] = actionCode;
            return message;
        }

        /// <summary>
        /// Get action code from message 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>action code</returns>
        public static int GetActionCode(this Message message)
            => message.MetaData.TryGetValue("ActionCode", out object actionCode) ? actionCode as int? ?? 0 : throw new Exception("Message doesn't have an action code");

        /// <summary>
        /// Get action code from received message and set message attribute + remove action code from data. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>message with an action code attribute</returns>
        internal static Message ProcessActionMessage(this Message message)
        {
            message.SetActionCode(BitConverter.ToInt32(message.Data, 0)); 

            if (message.Data.Length > 4) // Remove action code from message
            {
                var data = new byte[message.Data.Length - 4];
                Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
                message.Data = data;
            }
            else message.Data = null;
            
            return message;
        }
    }
}
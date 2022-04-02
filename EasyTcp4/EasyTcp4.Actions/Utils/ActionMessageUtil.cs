using System;

namespace EasyTcp4.Actions.Utils
{
    public static class ActionMessageUtil
    {
        /// <summary>
        /// Store the value of an action code inside a message
        /// </summary>
        public static Message SetActionCode(this Message message, int actionCode)
        {
            message.MetaData["ActionCode"] = actionCode;
            return message;
        }

        /// <summary>
        /// Get action code from message 
        /// </summary>
        public static int GetActionCode(this Message message)
            => message.MetaData.TryGetValue("ActionCode", out object actionCode) ?
            actionCode as int? ?? 0 : throw new Exception("Message doesn't have an action code");

        /// <summary>
        /// Get action code from received message and set message attribute + remove action code from data
        /// </summary>
        internal static Message ConvertToActionMessage(this Message message)
        {
            message.SetActionCode(BitConverter.ToInt32(message.Data, 0));

            if (message.Data.Length <= 4) message.Data = null;
            else
            {
#if (NETCOREAPP3_1 || NET5_0 || NET6_0)
                message.Data = message.Data[4..]; // More optimized solution
#else
                var data = new byte[message.Data.Length - 4];
                Buffer.BlockCopy(message.Data, 4, data, 0, data.Length);
                message.Data = data;
#endif
            }

            return message;
        }
    }
}

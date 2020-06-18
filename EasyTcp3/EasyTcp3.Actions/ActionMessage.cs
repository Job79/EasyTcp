namespace EasyTcp3.Actions
{
    public class ActionMessage : Message
    {
        /// <summary>
        /// Action code of received message
        /// </summary>
        public int ActionCode { get; }

        /// <summary></summary>
        /// <param name="message">received message</param>
        /// <param name="actionCode">action code of received message</param>
        public ActionMessage(Message message, int actionCode) : base(message.Data, message.Client)
            => ActionCode = actionCode;
    }
}
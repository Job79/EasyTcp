namespace EasyTcp3.Logging
{
    //TODO Add documentation
    public class LoggingMessage : Message
    {
        public LoggingMessage(Message message, bool isIncoming) : base(message?.Data, message?.Client)
            => IsIncoming = isIncoming;

        public bool IsIncoming { get; }
        public bool IsOutgoing => !IsIncoming;
    }
}
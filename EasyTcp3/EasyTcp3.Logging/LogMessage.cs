using System;

namespace EasyTcp3.Logging
{
    /// <summary>
    /// Object that holds information for the log functions
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Type of log message
        /// </summary>
        public LoggingType Type { get; }
        /// <summary>
        /// EasyTcpClient or EasyTcpServer as object
        /// </summary>
        public object Sender { get; }
        
        /// <summary></summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        public LogMessage(Message message, LoggingType type, object sender)
        {
            Type = type;
            Message = message;
            Client = message.Client;
            Sender = sender;
        }
      
        /// <summary></summary>
        /// <param name="client"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        public LogMessage(EasyTcpClient client, LoggingType type, object sender)
        {
            Type = type;
            Client = client;
            Sender = sender;
        }

        /// <summary></summary>
        /// <param name="exception"></param>
        /// <param name="sender"></param>
        public LogMessage(Exception exception, object sender)
        {
            Type = LoggingType.Error;
            Exception = exception;
            Sender = sender;
        }

        /*
         * Fields with information,
         * may be null depending on logMessage type
         */
        public Message Message { get; }
        public EasyTcpClient Client { get; }
        public Exception Exception { get; }

        /// <summary>
        /// Convert logMessage to string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => LoggingOutputGenerator.GenerateLog(this);
    }

    /// <summary>
    /// Different types of log-messages
    /// </summary>
    public enum LoggingType
    {
        DataReceived,
        DataSend,
        ClientConnected,
        ClientDisconnected,
        Error
    }
}
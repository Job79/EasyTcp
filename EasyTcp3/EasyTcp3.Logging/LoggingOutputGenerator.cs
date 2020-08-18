using System;
using System.Collections.Generic;
using System.Text;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;

namespace EasyTcp3.Logging
{
    /// <summary>
    /// Helper methods for converting a logMessage to a string
    /// </summary>
    internal static class LoggingOutputGenerator
    {
        /// <summary>
        /// Generation log form 1 message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static string GenerateLog(LogMessage message)
        {
            StringBuilder builder = new StringBuilder();
            if (message.Sender.GetType() == typeof(EasyTcpServer))
                builder.Append($"[Server {message.Sender.GetHashCode()}, ");
            else if (message.Sender.GetType() == typeof(EasyTcpClient))
                builder.Append($"[Client {message.Sender.GetHashCode()}, ");
            builder.Append($"{DateTime.Now:HH:mm:ss}] ");
            builder.Append(message.GetInfo());
            return builder.ToString();
        }

        /// <summary>
        /// Get information text from message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetInfo(this LogMessage message)
        {
            return message.Type switch
            {
                LoggingType.DataReceived =>
                $"[INFO] Received data [address: {message.Client.GetEndPoint()} message: \"{message.Message.ToString().Escape()}\" length: {message.Message.Data.Length} bytes]",
                LoggingType.DataSend =>
                $"[INFO] Send data [address: {message.Client.GetEndPoint()} message: \"{message.Message.ToString().Escape()}\" length: {message.Message.Data.Length} bytes]",
                LoggingType.ClientConnected =>
                $"[INFO] Connected [address: {message.Client.GetEndPoint()} id: {message.Client.GetHashCode()}]",
                LoggingType.ClientDisconnected => $"[INFO] Client disconnected [id: {message.Client?.GetHashCode()}]",
                LoggingType.Error => $"[ERROR] {message.Exception.Message} \n{message.Exception.StackTrace}",
                _ => "[ERROR] Unknown logging message"
            };
        }

        /// <summary>
        /// Escape characters that can not be printed to the console
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Escape(this string s)
        {
            StringBuilder builder = new StringBuilder(s);
            foreach (var chars in _escapeChars) builder.Replace(chars.Key, chars.Value);
            return builder.ToString();
        }
        
        /// <summary>
        /// Dictionary with unsafe characters
        /// </summary>
        private static Dictionary<string, string> _escapeChars = new Dictionary<string, string>
            {{"\r", "\\r"}, {"\n", "\\n"}, {"\t", "\\t"}, {"\a", "\\a"}, {"\b", "\\b"}, {"\f", "\\f"}};
    }
}
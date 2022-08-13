using System;
using System.Net;

namespace EasyTcp4.LogUtils
{
    public static class LogUtil
    {
        /// <summary>
        /// Enable logging
        /// </summary>
        public static T UseLogging<T>(this T client, Action<string> logger) where T : EasyTcpClient
        {
            client.OnConnect += (_, c) => logger(OnConnect(c));
            client.OnDisconnect += (_, c) => logger(OnDisconnect(c));
            client.OnDataSend += (_, m) => logger(OnDataSend(m));
            client.OnDataReceive += (_, m) => logger(OnDataReceive(m));
            client.OnError += (_, e) => logger(OnError(e));
            return client;
        }
        
        /// <summary>
        /// Enable logging
        /// </summary>
        public static T UseServerLogging<T>(this T server, Action<string> logger) where T : EasyTcpServer 
        {
            server.OnConnect += (_, c) => logger(OnConnect(c));
            server.OnDisconnect += (_, c) => logger(OnDisconnect(c));
            server.OnDataReceive += (_, m) => logger(OnDataReceive(m));
            server.OnDataSend += (_, m) => logger(OnDataSend(m));
            server.OnError += (_, e) => logger(OnError(e));
            return server;
        }

        /*
         * Logging messages
         */

        private static string OnConnect(EasyTcpClient c)
            => $"[INFO] Client connected [id: {c.GetHashCode()} address: {(IPEndPoint)c.BaseSocket.RemoteEndPoint}]";
        private static string OnDisconnect(EasyTcpClient c)
            => $"[INFO] Client disconnected [id: {c.GetHashCode()}]";
        private static string OnDataSend(Message m)
            => $"[INFO] Send data [id: {m.Client.GetHashCode()} message: \"{m}\" length: {m.Data.Length} bytes]";
        private static string OnDataReceive(Message m)
            => $"[INFO] Received data [id: {m.Client.GetHashCode()} message: \"{m}\" length: {m.Data.Length} bytes]";
        private static string OnError(Exception e)
            => $"[ERROR] {e.Message} \n{e.StackTrace}";
    }
}

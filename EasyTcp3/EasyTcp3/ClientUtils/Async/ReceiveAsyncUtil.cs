using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Class with async Receive functions
    /// </summary>
    public static class ReceiveAsyncUtil
    {
        /// <summary>
        /// Default timeout when timeout parameter is not specified 
        /// </summary>
        private const int DefaultTimeout = -1;

        /// <summary>
        /// Return next received data 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeout">maximum time to wait for a message, if time expired this function returns null</param>
        /// <returns>received message</returns>
        public static async Task<Message> ReceiveAsync(this EasyTcpClient client, TimeSpan? timeout = null)
        {
            Message reply = null;
            using var signal = new SemaphoreSlim(0, 1); // Use SemaphoreSlim as async ManualResetEventSlim

            client.DataReceiveHandler = message =>
            {
                reply = message;
                client.ResetDataReceiveHandler();
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Release();
                return Task.CompletedTask;
            };

            await signal.WaitAsync(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.ResetDataReceiveHandler();
            return reply;
        }
    }
}
using System;
using System.Threading;

namespace EasyTcp3.Test
{
    /// <summary>
    /// Class with basic functions to help with the unit tests
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Give every test an unique port,
        /// ! not thread safe
        /// </summary>
        private static int _portCounter = 1200;

        public static ushort GetPort() => (ushort) Math.Min(Interlocked.Increment(ref _portCounter), ushort.MaxValue);

        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Wait until default timeout expires, or continue if passed function returns true
        /// </summary>
        /// <param name="d"></param>
        public static void WaitWhileFalse(Func<bool> d)
        {
            for (int i = 0; i < DefaultTimeout.TotalMilliseconds && !d(); i += 10)
                Wait(10);
        }

        /// <summary>
        /// Wait until default timeout expires, or continue if passed function returns false
        /// </summary>
        /// <param name="d"></param>
        public static void WaitWhileTrue(Func<bool> d)
        {
            for (int i = 0; i < DefaultTimeout.TotalMilliseconds && d(); i += 10)
                Wait(10);
        }

        /// <summary>
        /// Wait x milliseconds
        /// </summary>
        /// <param name="milliseconds"></param>
        private static void Wait(int milliseconds) => Thread.Sleep(milliseconds);
    }
}

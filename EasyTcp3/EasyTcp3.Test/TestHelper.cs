using System;
using System.Threading;

namespace EasyTcp3.Test
{
    public static class TestHelper
    {
        private static int _portCounter = 1200;
        public static ushort GetPort() => (ushort) Math.Min(Interlocked.Increment(ref _portCounter), ushort.MaxValue);

        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

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
        private static void Wait(int milliseconds)
            => Thread.Sleep(milliseconds);
    }
}
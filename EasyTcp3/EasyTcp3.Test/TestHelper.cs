using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTcp3.Test
{
    public static class TestHelper
    {
        private static int _portCounter = 1200;
        public static ushort GetPort() => (ushort)Math.Min(Interlocked.Increment(ref _portCounter), ushort.MaxValue);
        
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Wait until default timeout expires, or passed functions returns true
        /// </summary>
        /// <param name="d"></param>
        public static void Wait(Func<bool> d)
        {
            for (int i = 0; i < DefaultTimeout.TotalMilliseconds && !d(); i += 10)
                Wait(TimeSpan.FromMilliseconds(10));
        }

        /// <summary>
        /// Wait x time
        /// </summary>
        /// <param name="time"></param>
        public static void Wait(TimeSpan time)
        {
            Task.Delay(time).Wait();
        }
    }
}
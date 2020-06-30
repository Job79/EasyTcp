using EasyTcp3.Examples.LargeArray;
using EasyTcp3.Examples.SpeedTest;
using EasyTcp3.Examples.Streams;

namespace EasyTcp3.Examples
{
    class Program
    {
        static void Main()
        {
            StreamExample.Run();
            StreamExample.Connect();
            // Run test methods here
            //MultiThreadedActionSpeedTest.Run();
        }
    }
}
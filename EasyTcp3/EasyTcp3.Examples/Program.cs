using System;
using EasyTcp3.Examples.SpeedTest;

namespace EasyTcp3.Examples
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press enter to start MultiThreaded speedtest");
            Console.ReadLine();
            MultiThreadedSpeedTest.Run();
        }
    }
}
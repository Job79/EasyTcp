using System;
using System.Threading.Tasks;
using EasyTcp3.Examples.Actions;
using EasyTcp3.Examples.Basic;
using EasyTcp3.Examples.Encryption;
using EasyTcp3.Examples.Files;
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
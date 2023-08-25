using Miri.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Miri
{
    public class Program
    {
        static void Main(string[] args)
        {
            Task.Delay(1000).Wait();
            new Core();
            Console.WriteLine("Done!");
            Task.Delay(5000).Wait();
        }
    }
}
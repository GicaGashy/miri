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
            Console.WriteLine("Starting...");
            
            Task.Delay(1000).Wait();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("This will take some time please wait!");
            Task.Delay(1000).Wait();
            var core = new Core();

            Console.WriteLine("Conversion done!");
            Console.WriteLine("Total files found: {0}. \nTotal files converted: {1}", core.TotalFiles, core.CurrentFile);
            Console.WriteLine("You can find your converted files at: {0}", core.prep.IoMap.ExportFolder);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Task.Delay(1000).Wait();
        }
    }
}
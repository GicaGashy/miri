using Miri.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Logic
{
    public class Prep
    {
        public InputOutputMap IoMap { get; set; }
        public Prep()
        {
            OpenIOMaps();
        }

        private void OpenIOMaps()
        {
            try
            {
                StreamReader r = new StreamReader("IOMaps.json");
                string json = r.ReadToEnd();
                IoMap = JsonConvert.DeserializeObject<InputOutputMap>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

            using (StreamReader r = new StreamReader("IOMaps.json"))
            {
                string json = r.ReadToEnd();
                IoMap = JsonConvert.DeserializeObject<InputOutputMap>(json);
            }
        }
    }
}

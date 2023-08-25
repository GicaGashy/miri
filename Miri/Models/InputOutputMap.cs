using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Models
{
    public class InputOutputMap
    {
        public string ImportFolder { get; set; }
        public string ExportFolder { get; set; }
        public Dictionary<string, string> CodeMaps { get; set; }

        public InputOutputMap() { 
            CodeMaps = new Dictionary<string, string>();
        }
    }
}

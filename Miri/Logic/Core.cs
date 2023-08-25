using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Logic
{
    public class Core
    {
        private Prep prep;
        public Core()
        {
            Run();
        }

        private List<string> ExtractPaths(string folderPath, string extension, List<string> filePaths = null)
        {
            if (filePaths == null)
            {
                filePaths = new List<string>();
            }

            foreach (string file in Directory.GetFiles(folderPath))
            {
                if (Path.GetExtension(file).Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    filePaths.Add(file);
                }
            }

            foreach (string subFolder in Directory.GetDirectories(folderPath))
            {
                ExtractPaths(subFolder, extension, filePaths);
            }

            return filePaths;
        }

        public void Run()
        {
            prep = new Prep();

            List<string> filePaths = ExtractPaths(prep.IoMap.ImportFolder, ".xxl");
            foreach (string filePath in filePaths)
            {
                new Conversion(filePath, prep);
            }
        }

    }
}

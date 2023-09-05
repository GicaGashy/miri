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
        public Prep prep;
        public int TotalFiles { get; set; }
        public int CurrentFile { get; set; } = 0;

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
                    Console.WriteLine(file);
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
            Task.Delay(1000).Wait();
            
            if(filePaths.Count == 0)
            {
                Console.WriteLine("No files found!");
                return;
            }

            TotalFiles = filePaths.Count;
            Console.WriteLine("Total files found: {0}", TotalFiles);

            foreach (string filePath in filePaths)
            {
                try
                {
                    var conversion = new Conversion(prep);
                    
                    var conversionResult = conversion.Run(filePath);

                    if (conversionResult)
                    {
                        Console.WriteLine("File {0} of {1} processed!", ++CurrentFile, TotalFiles);
                    
                        var copyPngResult = conversion.CopyPng(filePath);
                        
                        if (copyPngResult)
                        {
                            Console.WriteLine("PNG Copied!");
                        }
                    }

                } catch(Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

    }
}

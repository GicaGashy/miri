using Miri.Common;
using Miri.Contstants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miri.Logic
{
    public class Conversion
    {
        public Prep Prep;
        public decimal DzValue { get; set; } = 0.0m;
        public string LatestCValue { get; set; }
        public bool FirstLine { get; set; } = true;
        public Conversion(string filePath, Prep prep)
        {
            this.Prep = prep;
            Run(filePath);
        }

        public void Run(string filePath)
        {
            try
            {
                string outputFileName = Path.GetFileName(filePath);
                string relativePath = UtilMethods.StringMethods.GetRelativePath(filePath, Prep.IoMap.ImportFolder);
                string outputPath = Path.Combine(Prep.IoMap.ExportFolder, relativePath);

                string exportDirectory = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(exportDirectory);

                using (StreamReader reader = new StreamReader(filePath))
                using (StreamWriter writer = new StreamWriter(outputPath, false))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        writer.WriteLine(ProcessLine(line));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public string ProcessLine(string inputLine)
        {

            if (FirstLine)
            {
                // this is for the first line only
                Match dzMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.DZ_PATTERN);

                if (dzMatch.Success)
                {
                    DzValue = decimal.Parse(dzMatch.Groups[0].Value.Split('=')[1]);
                }

                this.FirstLine = false;

                return inputLine;
            }

            // this is for all lines
            foreach(KeyValuePair<string, string> kvp in Prep.IoMap.CodeMaps)
            {
                inputLine = inputLine.Replace(kvp.Key, kvp.Value);
            }

            Match cMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.C_PATTERN);

            if(cMatch.Success)
            {
                LatestCValue = cMatch.Groups[0].Value.Replace(" ", "");
                inputLine = "";
            }

            // lets do the C Replacement:
            Match tMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.T_PATTERN);
            if(tMatch.Success)
            {
                string tmp = tMatch.Groups[0].Value;
                inputLine = inputLine.Replace(tmp, tmp + " " + LatestCValue);
            }

            // lest do the DZ Replacement:
            Match xdzMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.XDZ_PATTERN);
            if(xdzMatch.Success)
            {
                string xdz = xdzMatch.Groups[0].Value;
                string zdzSplitted = xdz.Split('/')[1];
                int parsetInt = Int32.Parse(zdzSplitted);

                int calculatedDzValue = (int) DzValue / parsetInt;

                inputLine = inputLine.Replace(xdz, "X=" + calculatedDzValue.ToString());
                //inputLine = inputLine.Replace(tmp, tmp.Replace("DZ", DzValue.ToString()));
            }


            string outputLine = inputLine;

            return outputLine;
        }
    }
}

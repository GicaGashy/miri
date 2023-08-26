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

        private string ProcessLine(string inputLine)
        {

            if (FirstLine)
            {
                // this is for the first line only
                Match dzMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.DZ_PATTERN);

                if (dzMatch.Success)
                {
                    try
                    {
                        DzValue = decimal.Parse(dzMatch.Groups[0].Value.Split('=')[1]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred at dzMatch conversion part: {ex.Message}");
                    }

                }

                this.FirstLine = false;

                return inputLine;
            }

            // this is for all lines
            foreach(KeyValuePair<string, string> kvp in Prep.IoMap.CodeMaps)
            {
                try
                {
                    inputLine = inputLine.Replace(kvp.Key, kvp.Value);
                }
                catch (Exception ex) { 
                    Console.WriteLine($"An error occurred at KV Part (dynamic rules): {ex.Message}"); 
                }
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
                try
                {

                    string xdz = xdzMatch.Groups[0].Value;
                    string zdzSplitted = xdz.Split('/')[1];
                    int parsetInt = Int32.Parse(zdzSplitted);

                    int calculatedDzValue = (int) DzValue / parsetInt;

                    inputLine = inputLine.Replace(xdz, "X=" + calculatedDzValue.ToString());
                } catch (Exception e)
                {
                    Console.WriteLine($"An error occurred at xdzMatch conversion part: {e.Message}");
                }
            }

            // The z pattern catch: Z=-2.00+20.00
            Match zMatch = Regex.Match(inputLine, StringConstants.RegexPatterns.Z_PATTERN);
            if (zMatch.Success)
            {
                try
                {
                    string z = zMatch.Groups[0].Value;
                    inputLine = inputLine.Replace(z, zCatchLogic(z));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred at zMatch conversion part: {e.Message}");
                }
            }

            string outputLine = inputLine;

            return outputLine;
        }

        private string zCatchLogic(string input)
        {
            string rule = Prep.IoMap.ExceptionRules.ZRule1;

            // Z=-2.00+20.00
            if (rule == "OVERRIDE")
            {
                string tmp = input.Split('+')[1]; // 20.00
                return "Z=" + tmp;
            }

            if(rule == "CALCULATE")
            {
                // Z=-2.00+20.00
                string[] splitted= input.Split('=');

                string numberPart = splitted[1]; // -2.00+20.00
                string firstPart = numberPart.Split('+')[0]; // -2.00
                string secondPart = numberPart.Split('+')[1]; // 20.00

                decimal firstPartDecimal = decimal.Parse(firstPart);
                decimal secondPartDecimal = decimal.Parse(secondPart);

                decimal result = firstPartDecimal + secondPartDecimal;

                return "Z=" + result.ToString();
            }

            return input;
        }
    }
}

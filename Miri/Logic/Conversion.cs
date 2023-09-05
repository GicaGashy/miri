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
        private readonly Prep Prep;
        private decimal DzValue { get; set; } = 0.0m;
        private string LatestCValue { get; set; }
        private bool FirstLine { get; set; } = true;
        public Conversion(Prep prep)
        {
            this.Prep = prep;
        }

        public bool Run(string filePath)
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
                throw ex;
            }

            return true;
        }

        public bool CopyPng(string filePath)
        {
            try
            {
                string outputFileName = Path.GetFileNameWithoutExtension(filePath); // Get the file name without extension
                string directoryPath = Path.GetDirectoryName(filePath); // Get the directory path
                string pngFileName = $"{outputFileName}.png"; // Construct the .png file name

                // Check if a .png file with the same name exists in the same directory
                string[] pngFiles = Directory.GetFiles(directoryPath, pngFileName);

                if (pngFiles.Length > 0)
                {
                    // Get the first matching .png file
                    string pngFilePath = pngFiles[0];

                    // Construct the destination path in the export folder
                    string relativePath = UtilMethods.StringMethods.GetRelativePath(pngFilePath, Prep.IoMap.ImportFolder);
                    string destinationPath = Path.Combine(Prep.IoMap.ExportFolder, relativePath);

                    // Ensure the destination directory exists
                    string destinationDirectory = Path.GetDirectoryName(destinationPath);
                    Directory.CreateDirectory(destinationDirectory);

                    // Copy the .png file to the destination path
                    File.Copy(pngFilePath, destinationPath, true); // The 'true' flag allows overwriting if the file already exists
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return true;
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
                        var convertedDecimal = UtilMethods.Conversions.StringToDecimal(dzMatch.Groups[1].Value);
                        
                        DzValue = convertedDecimal;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred at dzMatch conversion part: {ex.Message}");
                        throw ex;
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

                    string xdz = xdzMatch.Groups[0].Value; // "Y=DZ/2"
                    
                    string zdsSplittedVariablePart = xdz.Split('=')[0]; // "Y"
                    string xdzSplittedValuePart = xdz.Split('/')[1]; // "2"
                    int parsetInt = UtilMethods.Conversions.StringToInt(xdzSplittedValuePart);

                    int calculatedDzValue = (int) DzValue / parsetInt;

                    inputLine = inputLine.Replace(xdz, $"{zdsSplittedVariablePart}={calculatedDzValue}");
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
                    inputLine = inputLine.Replace(z, ZCatchLogic(z));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred at zMatch conversion part: {e.Message}");
                }
            }

            string outputLine = inputLine;

            return outputLine;
        }

        private string ZCatchLogic(string input)
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

                decimal firstPartDecimal =  UtilMethods.Conversions.StringToDecimal(firstPart);
                decimal secondPartDecimal = UtilMethods.Conversions.StringToDecimal(secondPart);

                decimal result = firstPartDecimal + secondPartDecimal;

                return "Z=" + result.ToString();
            }

            return input;
        }

    }
}

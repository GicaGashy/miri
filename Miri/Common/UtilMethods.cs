using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Common
{
    public static class UtilMethods
    {
        public static class StringMethods
        {
            public static string GetRelativePath(string fullPath, string basePath)
            {
                Uri fullPathUri = new Uri(fullPath);
                Uri basePathUri = new Uri(basePath);

                Uri relativeUri = basePathUri.MakeRelativeUri(fullPathUri);
                return Uri.UnescapeDataString(relativeUri.ToString());
            }
        }

        public static class Conversions
        {
            public static decimal StringToDecimal(string str, string cultureInfoCode = "en-US")
            {
                try
                {
                    var cultureInfo = new CultureInfo(cultureInfoCode);
                    return Convert.ToDecimal(str, cultureInfo);

                } catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }

            public static int StringToInt(string str, string cultureInfoCode = "en-US")
            {
                try
                {
                    var cultureInfo = new CultureInfo(cultureInfoCode);
                    return Convert.ToInt32(str, cultureInfo);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miri.Contstants
{
    public static class StringConstants
    {
        public static class RegexPatterns
        {
            public const string DZ_PATTERN  = @"DZ=([\d.]+)";
            public const string C_PATTERN   = @"C\s*=\s*(\d+)";
            public const string T_PATTERN   = @"T=\d+";
            public const string XDZ_PATTERN = @"(X|Y)=DZ\/[\d.]+";
            public const string Z_PATTERN = @"Z=-\d+\.\d+\+\d+\.\d+"; // complex: @"Z=(-|\+)?\d+\.\d+(-|\+)\d+\.\d+";
        }
    }
}

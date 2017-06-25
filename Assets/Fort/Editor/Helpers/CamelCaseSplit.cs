using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort.Inspector
{
    internal class CamelCaseSplit
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1").Trim();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dziennik
{
    public static class Ext
    {
        public static string ValueOrDefault(this XElement element, string def)
        {
            return (element.Value == null ? def : element.Value);
        }

        public static bool BoolParseOrDefault(string input, bool def)
        {
            bool output;
            if (bool.TryParse(input, out output))
            {
                return output;
            }
            return def;
        }
    }
}

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

        public static decimal DecimalRoundHalfUp(decimal d)
        {
            return DecimalRoundHalfUp(d, 0);
        }
        //http://stackoverflow.com/questions/311696/why-does-net-use-bankers-rounding-as-default
        public static decimal DecimalRoundHalfUp(decimal d, int decimals)
        {
            if (decimals < 0)
            {
                throw new ArgumentException("The decimals must be non-negative",
                    "decimals");
            }

            decimal multiplier = (decimal)Math.Pow(10, decimals);
            decimal number = d * multiplier;

            if (decimal.Truncate(number) < number)
            {
                number += 0.5m;
            }
            return decimal.Round(number) / multiplier;
        }
    }
}

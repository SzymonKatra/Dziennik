using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dziennik
{
    public static class Extensions
    {
        public static string ValueOrDefault(this XElement element, string def)
        {
            return (element.Value == null ? def : element.Value);
        }
    }
}

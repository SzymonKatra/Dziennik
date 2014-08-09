using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

        public static string RemoveAllWhitespaces(string value)
        {
            if (value == null) return value;

            value = value.Replace(" ", "");
            value = value.Replace("\t", "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");

            return value;
        }

        //private static BinaryFormatter s_formatter = new BinaryFormatter();
        //public static T DeepClone<T>(T original)
        //{
        //    T result; 
        //    using(MemoryStream stream = new MemoryStream())
        //    {
        //        s_formatter.Serialize(stream, original);
        //        stream.Position = 0;
        //        result = (T)s_formatter.Deserialize(stream);
        //    }
        //    return result;
        //}
    }
}

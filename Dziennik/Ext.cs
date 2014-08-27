using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

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
        public static int IntParseOrDefault(string input, int def)
        {
            int output;
            if (int.TryParse(input, out output))
            {
                return output;
            }
            return def;
        }
        public static long LongParseOrDefault(string input, long def)
        {
            long output;
            if (long.TryParse(input, out output))
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

        public static void ClearDirectory(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (var file in info.GetFiles()) file.Delete();
            foreach (var sub in info.GetDirectories()) sub.Delete(true);
        }

        //thanks to: SwDevMan81
        //http://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality
        public static bool IsEqualTo(this SecureString ss1, SecureString ss2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(ss1);
                bstr2 = Marshal.SecureStringToBSTR(ss2);
                int length1 = Marshal.ReadInt32(bstr1, -4);
                int length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (int x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class Color : ModelBase
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            Color result = new Color();

            result.A = a;
            result.R = r;
            result.G = g;
            result.B = b;

            return result;
        }
    }
}

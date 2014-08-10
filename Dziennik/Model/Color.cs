using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public struct Color
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

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

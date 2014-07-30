using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Dziennik.Controls
{
    public class DecimalToBrushConverter : IValueConverter
    {
        private struct ColorDecimal
        {
            public decimal R;
            public decimal G;
            public decimal B;
            public decimal A;

            public ColorDecimal(decimal r, decimal g, decimal b, decimal a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public static ColorDecimal FromColor(Color color)
            {
                ColorDecimal result = new ColorDecimal();

                result.R = (decimal)color.R / 255M;
                result.G = (decimal)color.G / 255M;
                result.B = (decimal)color.B / 255M;
                result.A = (decimal)color.A / 255M;

                return result;
            }
            public Color ToColor()
            {
                Color result = new Color();

                result.R = (byte)(this.R * 255M);
                result.G = (byte)(this.G * 255M);
                result.B = (byte)(this.B * 255M);
                result.A = (byte)(this.A * 255M);

                return result;
            }

            public static ColorDecimal operator+(ColorDecimal a, ColorDecimal b)
            {
                return new ColorDecimal(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
            }
            public static ColorDecimal operator -(ColorDecimal a, ColorDecimal b)
            {
                return new ColorDecimal(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
            }
            public static ColorDecimal operator *(ColorDecimal a, decimal b)
            {
                return new ColorDecimal(a.R * b, a.G *b, a.B *b, a.A * b);
            }
            public static ColorDecimal operator /(ColorDecimal a, decimal b)
            {
                return new ColorDecimal(a.R / b, a.G / b, a.B / b, a.A / b);
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = (string)parameter;
            string[] tokens = parameterString.Split('-');
            decimal low = decimal.Parse(tokens[0], CultureInfo.InvariantCulture);
            decimal high = decimal.Parse(tokens[1], CultureInfo.InvariantCulture); 
            bool lowerRangeRed = bool.Parse(tokens[2]);

            decimal input = (decimal)value;
            if ((input <= low && lowerRangeRed) || input < low) return Brushes.Red;

            input -= low;
            //input *= 2;

            ColorDecimal lowColor = ColorDecimal.FromColor(Colors.Orange);
            ColorDecimal highColor = ColorDecimal.FromColor(Colors.LightGreen);

            ColorDecimal result = (highColor - lowColor) * (input / (high - low)) + lowColor;

            return new SolidColorBrush(result.ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

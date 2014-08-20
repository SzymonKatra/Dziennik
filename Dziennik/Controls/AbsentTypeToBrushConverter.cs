using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Dziennik.Model;

namespace Dziennik.Controls
{
    public class AbsentTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PresenceType presence = (PresenceType)value;

            if (presence == PresenceType.Absent)
                return Brushes.Red;
            else if (presence == PresenceType.AbsentJustified)
                return Brushes.LightGreen;
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartWard.Converters
{
    public class TimelineColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((SmartWard.Models.MessageFlags)value)
            {
                case Models.MessageFlags.Comment:
                    return "White";
                case Models.MessageFlags.Event:
                    return "Blue";
                case Models.MessageFlags.Surgery:
                    return "Red";
                case Models.MessageFlags.In:
                    return "yellow";
                case Models.MessageFlags.Out:
                    return "Green";
            }
            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

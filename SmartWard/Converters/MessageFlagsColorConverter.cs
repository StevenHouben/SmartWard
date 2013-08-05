using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Converters
{
    public class MessageFlagsColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((SmartWard.Models.MessageFlags)value)
            {
                case Models.MessageFlags.Comment:
                    return Brushes.White;
                case Models.MessageFlags.Event:
                    return Brushes.LightBlue;
                case Models.MessageFlags.Surgery:
                    return Brushes.LightSalmon;
                case Models.MessageFlags.In:
                    return Brushes.LightGoldenrodYellow;
                case Models.MessageFlags.Out:
                    return Brushes.LightGreen;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Brushes.Transparent;
        }
    }
}

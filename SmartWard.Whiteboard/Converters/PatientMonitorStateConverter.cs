using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Whiteboard.Converters
{
    internal class PatientMonitorStateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 1:
                    return new SolidColorBrush(Colors.Black);
                case 2:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var col = (Color)value;
            if (col == Colors.Red) return 2;
            return col == Colors.Black ? 1 : 0;
        }
    }
}

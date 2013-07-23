using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Converters
{
    public class PatientStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 1:
                    return new SolidColorBrush(Colors.Red);
                case 2:
                    return new SolidColorBrush(Colors.Green);
                case 3: 
                    return new SolidColorBrush(Colors.Blue);
                case 4:
                    return new SolidColorBrush(Colors.Yellow);
                case 5:
                    return new SolidColorBrush(Colors.Cyan);
                case 6:
                    return new SolidColorBrush(Colors.Magenta);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var col = (Color)value;
            if (col == Colors.Red) return 1;
            else if (col == Colors.Green) return 2;
            else if (col == Colors.Blue) return 3;
            else if (col == Colors.Yellow) return 4;
            else if (col == Colors.Cyan) return 5;
            else if (col == Colors.Magenta) return 6;
            else return 7;
        }
    }
}

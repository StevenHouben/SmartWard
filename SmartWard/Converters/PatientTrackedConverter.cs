using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Converters
{
    public class PatientTrackedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.White;
            var selected = (bool)value;
            return selected ? new SolidColorBrush(Colors.LightYellow) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = ((SolidColorBrush)value);
            return false;
        }
    }
}

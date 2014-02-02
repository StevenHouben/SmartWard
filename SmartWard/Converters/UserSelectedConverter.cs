using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Converters
{
    public class UserSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.Aquamarine;
            var selected = (bool)value;
            return selected ? new SolidColorBrush(Colors.MediumAquamarine) : new SolidColorBrush(Colors.Aquamarine);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color == Colors.MediumAquamarine ? true : false;
        }
    }
}

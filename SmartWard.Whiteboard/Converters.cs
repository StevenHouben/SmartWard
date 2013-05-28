using SmartWard.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartWard.Whiteboard
{
    public class Converters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.Magenta;
            var color = (RGB)value;
            return new SolidColorBrush(Color.FromArgb(255,color.Red, color.Green, color.Blue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = ((SolidColorBrush)value).Color;
            return new RGB(color.R, color.G, color.B);
        }
    }
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.Magenta;
            var selected = (bool)value;
            return selected ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = ((SolidColorBrush)value);
            return false;
        }
    }
}

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
    public class StatusConverter : IValueConverter
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
                    return new SolidColorBrush(Colors.LightBlue);
                case 6:
                    return new SolidColorBrush(Colors.Magenta);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var col = (Color)value;
            if(col == Colors.Red) return 1;
            if(col ==  Colors.Green) return 2;
            else if(col ==  Colors.Blue) return 3;
            else if(col ==  Colors.Yellow) return 4;
            else if(col ==  Colors.LightBlue) return 5;
            else if(col ==  Colors.Magenta) return 6;
            else return 7;
        }
    }
    public class ValueConverter : IValueConverter
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
            return selected ? new SolidColorBrush(Colors.LightYellow) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = ((SolidColorBrush)value);
            return false;
        }
    }
}

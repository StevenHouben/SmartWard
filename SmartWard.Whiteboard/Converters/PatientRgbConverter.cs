using System;
using System.Windows.Data;
using System.Windows.Media;
using ABC.Model.Primitives;
using Brushes = System.Drawing.Brushes;

namespace SmartWard.Whiteboard.Converters
{
    class PatientRgbConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.Magenta;
            var color = (Rgb)value;
            return new SolidColorBrush(Color.FromArgb(255,color.Red, color.Green, color.Blue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = ((SolidColorBrush)value).Color;
            return new Rgb(color.R, color.G, color.B);
        }
    }
}

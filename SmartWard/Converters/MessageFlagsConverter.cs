using SmartWard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartWard.Converters
{
    public class MessageFlagsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return MessageFlags.Comment.ToString();
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return MessageFlags.Comment;
            var flag = Enum.Parse(typeof(MessageFlags), value as string);
            return flag;
        }
    }
}

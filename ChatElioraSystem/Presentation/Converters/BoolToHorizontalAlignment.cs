using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ChatElioraSystem.Presentation.Converters
{
    internal class BoolToHorizontalAlignment : IValueConverter
    {       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUser)
                return isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            return HorizontalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

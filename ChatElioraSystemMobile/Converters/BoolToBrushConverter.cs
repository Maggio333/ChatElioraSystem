using System.Globalization;

namespace ChatElioraSystemMobile.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush UserBrush { get; set; } = new SolidColorBrush(Color.FromRgb(31, 9, 7));  // Błękit użytkownika
        public Brush BotBrush { get; set; } = new SolidColorBrush(Color.FromRgb(13, 20, 28));    // Głębia Eliory

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUser)
                return isUser ? UserBrush : BotBrush;
            return BotBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

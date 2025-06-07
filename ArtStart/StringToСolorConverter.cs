using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace ArtStart
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexColor)
            {
                return new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(hexColor)
                );
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
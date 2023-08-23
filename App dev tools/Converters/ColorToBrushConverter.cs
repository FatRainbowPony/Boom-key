using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AppDevTools.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        #region Methods

        #region Public
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace AppDevTools.Converters
{
    public class TextToUpperConverter : IValueConverter
    {
        #region Methods

        #region Public
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : ((string)value).ToUpper();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
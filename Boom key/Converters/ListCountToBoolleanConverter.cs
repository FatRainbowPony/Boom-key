using System;
using System.Globalization;
using System.Windows.Data;

namespace BoomKey.Converters
{
    public class ListCountToBoolleanConverter : IValueConverter
    {
        #region Methods

        #region Public
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            if (count == 0)
            {
                return false;
            }

            return true;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
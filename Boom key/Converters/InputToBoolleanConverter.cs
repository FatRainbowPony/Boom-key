using System;
using System.Windows.Data;

namespace BoomKey.Converters
{
    public class InputToBoolleanConverter : IValueConverter
    {
        #region Methods

        #region Public
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Addons.MultModifierKey multModifierKey = (Addons.MultModifierKey)value;
            if (multModifierKey == Addons.MultModifierKey.None)
            {
                return false;
            }

            return true;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
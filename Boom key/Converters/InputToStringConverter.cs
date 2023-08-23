using System;
using System.Windows.Data;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.Converters
{
    public class InputToStringConverter : IValueConverter
    {
        #region Methods

        #region Public
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string? inputAsStr = null;

            switch (value)
            {
                case Addons.Key key:
                    inputAsStr = appDevToolsExts.Enum.GetMemberValue(key);
                    break;

                case Addons.MultModifierKey multModifierKey:
                    inputAsStr = appDevToolsExts.Enum.GetMemberValue(multModifierKey);
                    break;
            }

            return inputAsStr;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
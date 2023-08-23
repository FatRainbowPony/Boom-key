using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace BoomKey.Addons
{
    public static class Colors
    {
        #region Fields

        #region Private
        private static Color white = Color.FromRgb(255, 255, 255);
        private static Color lightYellow = Color.FromRgb(255, 255, 128);
        private static Color yellow = Color.FromRgb(255, 255, 0);
        private static Color lightOrange = Color.FromRgb(255, 128, 64);
        private static Color orange = Color.FromRgb(255, 128, 0);
        private static Color lightRed = Color.FromRgb(255, 128, 128);
        private static Color red = Color.FromRgb(255, 0, 0);
        private static Color pink = Color.FromRgb(255, 128, 192);
        private static Color darkPink = Color.FromRgb(255, 0, 128);
        private static Color purple = Color.FromRgb(128, 0, 255);
        private static Color darkPurple = Color.FromRgb(128, 0, 128);
        private static Color blue = Color.FromRgb(0, 128, 255);
        private static Color middleBlue = Color.FromRgb(0, 128, 192);
        private static Color darkBlue = Color.FromRgb(0, 64, 128);
        private static Color lightGreen = Color.FromRgb(128, 255, 128);
        private static Color middleGreen = Color.FromRgb(0, 255, 64);
        private static Color green = Color.FromRgb(0, 128, 0);
        private static Color darkGreen = Color.FromRgb(0, 128, 64);
        private static Color lightBrown = Color.FromRgb(126, 115, 95);
        private static Color middleBrown = Color.FromRgb(132, 117, 69);
        private static Color brown = Color.FromRgb(128, 64, 0);
        private static Color lightGray = Color.FromRgb(192, 192, 192);
        private static Color gray = Color.FromRgb(128, 128, 128);
        private static Color black = Color.FromRgb(0, 0, 0);
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static Color White
        {
            get { return white; }
        }

        public static Color LightYellow
        {
            get { return lightYellow; }
        }

        public static Color Yellow
        {
            get { return yellow; }
        }

        public static Color LightOrange
        {
            get { return lightOrange; }
        }

        public static Color Orange
        {
            get { return orange; }
        }

        public static Color LightRed
        {
            get { return lightRed; }
        }

        public static Color Red
        {
            get { return red; }
        }

        public static Color Pink
        {
            get { return pink; }
        }

        public static Color DarkPink
        {
            get { return darkPink; }
        }

        public static Color Purple
        {
            get { return purple; }
        }

        public static Color DarkPurple
        {
            get { return darkPurple; }
        }

        public static Color Blue
        {
            get { return blue; }
        }

        public static Color MiddleBlue
        {
            get { return middleBlue; }
        }

        public static Color DarkBlue
        {
            get { return darkBlue; }
        }

        public static Color LightGreen
        {
            get { return lightGreen; }
        }

        public static Color MiddleGreen
        {
            get { return middleGreen; }
        }

        public static Color Green
        {
            get { return green; }
        }

        public static Color DarkGreen
        {
            get { return darkGreen; }
        }

        public static Color LightBrown
        {
            get { return lightBrown; }
        }

        public static Color MiddleBrown
        {
            get { return middleBrown; }
        }

        public static Color Brown
        {
            get { return brown; }
        }

        public static Color LightGray
        {
            get { return lightGray; }
        }

        public static Color Gray
        {
            get { return gray; }
        }


        public static Color Black
        {
            get { return black; }
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Public
        public static List<Color> GetAll()
        {
            List<Color> colors = new();

            Type defColors = typeof(Colors);
            List<PropertyInfo> properties = defColors.GetProperties(BindingFlags.Public | BindingFlags.Static).ToList();
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(defColors);
                if (value != null && property.PropertyType == typeof(Color))
                {
                    colors.Add((Color)value);
                }
            }

            return colors;
        }
        #endregion Public

        #endregion Methods
    }
}
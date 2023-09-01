using System;
using System.Windows;
using System.Windows.Media;

namespace AppDevTools.Addons
{
    public static class Colors
    {
        #region Constants

        #region Private
        private const string DEF_ASSEMBLYNAME = "AppDevTools";
        private const string DEF_FOLDERNAME_THEME = "Themes";
        private const string DEF_FILENAME_GENERIC = "GenericDictionary.xaml";
        private const string DEF_FILENAME_UI = "UIStylesDictionary.xaml";
        private const string DEF_FILENAME_COLORS = "ColorsDictionary.xaml";
        private const string DEF_TITLEBAR_COLOR_NAME = "TitleBarBackgroundBrush";
        #endregion Private

        #endregion Constants

        #region Methods

        #region Private
        private static bool IsContainedGenericInPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                path.Contains(DEF_ASSEMBLYNAME) && path.Contains(DEF_FOLDERNAME_THEME) && path.Contains(DEF_FILENAME_GENERIC))
            {
                return true;
            }

            return false;
        }

        private static bool IsContainedUiInPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                path.Contains(DEF_ASSEMBLYNAME) && path.Contains(DEF_FOLDERNAME_THEME) && path.Contains(DEF_FILENAME_UI))
            {
                return true;
            }

            return false;
        }

        private static bool IsContainedColorsInPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                path.Contains(DEF_ASSEMBLYNAME) && path.Contains(DEF_FOLDERNAME_THEME) && path.Contains(DEF_FILENAME_COLORS))
            {
                return true;
            }

            return false;
        }
        #endregion Private

        #region Public
        public static ResourceDictionary? Get(Application app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            foreach (ResourceDictionary mainDictionary in app.Resources.MergedDictionaries)
            {
                if (mainDictionary.Source.IsAbsoluteUri && IsContainedGenericInPath(mainDictionary.Source.AbsolutePath))
                {
                    foreach (ResourceDictionary subDictionary in mainDictionary.MergedDictionaries)
                    {
                        if (subDictionary.Source.IsAbsoluteUri && IsContainedUiInPath(subDictionary.Source.AbsolutePath))
                        {
                            foreach (ResourceDictionary resultDictionary in subDictionary.MergedDictionaries)
                            {
                                if (resultDictionary.Source.IsAbsoluteUri && IsContainedColorsInPath(resultDictionary.Source.AbsolutePath))
                                {
                                    return resultDictionary;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static void SetTitleBarColor(Application app, SolidColorBrush titleBarColor)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (titleBarColor == null)
            {
                throw new ArgumentNullException(nameof(titleBarColor));
            }

            ResourceDictionary? colorsDictionary = Get(app);
            colorsDictionary?.Remove(DEF_TITLEBAR_COLOR_NAME);
            colorsDictionary?.Add(DEF_TITLEBAR_COLOR_NAME, titleBarColor);
        }
        #endregion Public

        #endregion Methods
    }
}
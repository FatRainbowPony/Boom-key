using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppDevTools.Addons
{
    /// <summary>
    /// Provides a collection of properties and methods for managing theme for application
    /// </summary>
    public class Theme
    {
        #region Constants

        #region Private
        private const string SYS_DIR_PERSONALIZATION = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
        private const string SYS_NAME_LIGHT_THEME = "SystemUsesLightTheme";
        private const string DEF_ASSEMBLYNAME = "AppDevTools";
        private const string DEF_FOLDERNAME_THEME = "Themes";
        private const string DEF_FILENAME_THEME = "GenericDictionary.xaml";
        #endregion Private

        #endregion Constants

        #region Enums

        #region Public
        /// <summary>
        /// The specific description that describe the theme
        /// </summary>
        public enum ThemeDescription
        {
            None,
            LightTheme,
            DarkTheme
        }
        #endregion Public

        #endregion Enums 

        #region Properties

        #region Public

        #region Properties for specific register key
        /// <summary>
        /// Gets the specific register key
        /// </summary>
        public RegistryKey? RegistryKey { get; private set; }
        #endregion Properties for specific register key

        #region Properties for theme description
        /// <summary>
        /// Gets or sets the theme description
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ThemeDescription Description { get; set; }
        #endregion Properties for theme description

        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public

        #region Constructor for creating base on system theme
        /// <summary>
        /// Initializes a new instance of the Theme class based on the system theme
        /// </summary>
        public Theme()
        {
            RegistryKey = Registry.CurrentUser.OpenSubKey(SYS_DIR_PERSONALIZATION);
            object? dataAboutSysTheme = null;

            if (RegistryKey != null)
            {
                dataAboutSysTheme = RegistryKey.GetValue(SYS_NAME_LIGHT_THEME);
                if (dataAboutSysTheme != null)
                {
                    int idSysTheme = (int)dataAboutSysTheme;
                    if (idSysTheme == 1)
                    {
                        Description = ThemeDescription.LightTheme;
                    }
                    else
                    {
                        Description = ThemeDescription.DarkTheme;
                    }
                }
            }

            if (RegistryKey == null || dataAboutSysTheme == null)
            {
                Description = ThemeDescription.None;
            }
        }
        #endregion Constructor for creating base on system theme

        #region Constructor for creating base on theme description
        /// <summary>
        /// Initializes a new instance of the Theme class with the specific theme description
        /// </summary>
        /// <param name="description">
        /// The theme description
        /// </param>
        public Theme(ThemeDescription description)
        {
            Description = description;
        }
        #endregion Constructor for creating base on theme description

        #endregion Public

        #endregion Construcotrs

        #region Methods

        #region Private

        #region Method for checking the theme is contained in path
        private static bool IsContainedThemeInPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                path.Contains(DEF_ASSEMBLYNAME) && path.Contains(DEF_FOLDERNAME_THEME) && path.Contains(DEF_FILENAME_THEME))
            {
                foreach (string themeDescription in Enum.GetNames(typeof(ThemeDescription)).ToList())
                {
                    if (path.Contains(themeDescription))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion Method for checking the theme is contained in path

        #region Method for getting uri string for theme
        private static string GetUriStringForTheme(ThemeDescription themeDescription)
        {
            return $"pack://application:,,,/{DEF_ASSEMBLYNAME};component/{DEF_FOLDERNAME_THEME}/{themeDescription}/{DEF_FILENAME_THEME}";
        }
        #endregion Method for getting uri string theme

        #endregion Private

        #region Public

        #region Method for checking the theme is installed in application resources
        /// <summary>
        /// Check the theme is installed in application resources
        /// </summary>
        /// <param name="app">
        /// The application in which the theme is checked
        /// </param>
        /// <param name="resourceDictionaries">
        /// Application resources where the theme is searched
        /// </param>
        /// <returns>
        /// true - is installed, otherwise false
        /// </returns>
        public static bool IsInstalled(Application app, List<ResourceDictionary> resourceDictionaries)
        {
            if (app != null && resourceDictionaries != null)
            {
                foreach (ResourceDictionary resourceDictionary in resourceDictionaries)
                {
                    if (resourceDictionary.Source.IsAbsoluteUri && IsContainedThemeInPath(resourceDictionary.Source.AbsolutePath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion Method for checking the theme is installed in application resources

        #region Method for checking the theme is equal in application resources 
        /// <summary>
        /// Checks the theme is equal in application resources
        /// </summary>
        /// <param name="app">
        /// The application in which the theme is checked
        /// </param>
        /// <param name="resourceDictionaries">
        /// Application resources where the theme is searched
        /// </param>
        /// <returns>
        /// true - is equal, otherwise false
        /// </returns>
        public bool IsEqual(Application app, List<ResourceDictionary> resourceDictionaries)
        {
            if (app != null && resourceDictionaries != null)
            {
                string absolutePath = string.Empty;

                foreach (ResourceDictionary resourceDictionary in resourceDictionaries)
                {
                    if (resourceDictionary.Source.IsAbsoluteUri && IsContainedThemeInPath(resourceDictionary.Source.AbsolutePath))
                    {
                        absolutePath = resourceDictionary.Source.AbsolutePath;

                        break;
                    }
                }

                if (!string.IsNullOrEmpty(absolutePath))
                {
                    if (absolutePath.Contains(Description.ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion Method for checking the theme is equal in application resources 

        #region Methods for getting theme

        #region Method for getting theme from system settings
        /// <summary>
        /// Gets theme from system settings
        /// </summary>
        /// <returns>
        /// The theme
        /// </returns>
        public static Theme Get()
        {
            return new Theme();
        }
        #endregion Method for getting theme from system settings

        #region Method for getting theme from file
        /// <summary>
        /// Gets theme from file
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file
        /// </param>
        /// <returns>
        /// The theme, or null if the file does't contain data about the theme
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Theme? Get(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException();
            }

            List<Theme>? content = Json.GetDeserializedObj<Theme>(File.ReadAllText(pathToFile));
            if (content != null)
            {
                return content.FirstOrDefault();
            }

            return null;
        }
        #endregion Method for getting theme from file

        #region Method for getting theme from application resources
        /// <summary>
        /// Gets theme from application resources
        /// </summary>
        /// <param name="app">
        /// The application from which the theme is getting
        /// </param>
        /// <param name="resourceDictionaries">
        /// Application resources where the theme is searched
        /// </param>
        /// <returns>
        /// The theme, or null if application resources doesn't contain data about the theme
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Theme? Get(Application app, List<ResourceDictionary> resourceDictionaries)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (resourceDictionaries == null)
            {
                throw new ArgumentNullException(nameof(resourceDictionaries));
            }

            foreach (ResourceDictionary resourceDictionary in resourceDictionaries)
            {
                if (resourceDictionary.Source.IsAbsoluteUri)
                {
                    ThemeDescription themeDescription = GetThemeDescriptionInPath(resourceDictionary.Source.AbsolutePath);
                    if (themeDescription != ThemeDescription.None)
                    {
                        return new Theme(themeDescription);
                    }
                }
            }

            return null;

            static ThemeDescription GetThemeDescriptionInPath(string path)
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                    path.Contains(DEF_ASSEMBLYNAME) && path.Contains(DEF_FOLDERNAME_THEME) && path.Contains(DEF_FILENAME_THEME))
                {
                    foreach (string themeDescription in Enum.GetNames(typeof(ThemeDescription)).ToList())
                    {
                        if (path.Contains(themeDescription))
                        {
                            bool isSuccessfullyParse = Enum.TryParse(themeDescription, out ThemeDescription description);
                            if (isSuccessfullyParse)
                            {
                                return description;
                            }
                        }
                    }
                }

                return ThemeDescription.None;
            }
        }
        #endregion Method for getting theme from application resources

        #endregion Method for getting theme

        #region Method for setting theme for application
        /// <summary>
        /// Sets theme for application
        /// </summary>
        /// <param name="app">
        /// The application for which the theme is being installed
        /// </param>
        /// <param name="resourceDictionaries">
        /// Application resources where a resource with a theme is installed
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(Application app, List<ResourceDictionary> resourceDictionaries)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (resourceDictionaries == null)
            {
                throw new ArgumentNullException(nameof(resourceDictionaries));
            }

            if (Description != ThemeDescription.None)
            {
                foreach (ResourceDictionary resourceDictionary in resourceDictionaries)
                {
                    if (resourceDictionary.Source.IsAbsoluteUri && IsContainedThemeInPath(resourceDictionary.Source.AbsolutePath))
                    {
                        app.Resources.MergedDictionaries.Remove(resourceDictionary);
                    }            
                }

                switch (Description)
                {
                    case ThemeDescription.LightTheme:
                        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(GetUriStringForTheme(Description)) });
                        break;

                    case ThemeDescription.DarkTheme:
                        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(GetUriStringForTheme(Description)) });
                        break;
                }
            }
        }
        #endregion Methods for setting theme for application

        #endregion Public

        #endregion Methods
    }
}
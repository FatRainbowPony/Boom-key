using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppDevTools.Addons;

namespace AppInstaller.Addons
{
    public class Lang : AppDevTools.Addons.Lang
    {
        #region Constants

        #region Private
        private const string DEF_LIBRARYNAME = "AppDevTools";
        private const string DEF_FOLDERNAME_TEXTS = "TextsDictionaries";
        private const string DEF_FILENAME_TEXTS = "TextsDictionary.xaml";
        #endregion Private

        #endregion Constants 

        #region Constructors

        #region Public
        public Lang() : base() { }

        public Lang(LangDescription description) : base(description) { }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private static bool IsContainedLangInPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) &&
                path.Contains(DEF_FOLDERNAME_TEXTS) && path.Contains(DEF_FILENAME_TEXTS))
            {
                foreach (string langDescription in Enum.GetNames(typeof(LangDescription)).ToList())
                {
                    if (path.Contains(langDescription))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetUriStringForLangInLib(LangDescription langDescription)
        {
            return $"pack://application:,,,/{DEF_LIBRARYNAME};component/Dictionaries/{DEF_FOLDERNAME_TEXTS}/{langDescription}/{DEF_FILENAME_TEXTS}";
        }

        private static string GetUriStringForLangInApp(LangDescription langDescription)
        {
            return $"pack://application:,,,/{App.Name};component/Dictionaries/{DEF_FOLDERNAME_TEXTS}/{langDescription}/{DEF_FILENAME_TEXTS}";
        }
        #endregion Private

        #region Public
        public void Set(Application app, List<ResourceDictionary> appResourceDictionaries)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (appResourceDictionaries == null)
            {
                throw new ArgumentNullException(nameof(appResourceDictionaries));
            }

            ResourceDictionary? libraryResourceDictionary = null;

            foreach (ResourceDictionary appResourceDictionary in appResourceDictionaries)
            {
                if (appResourceDictionary.Source.IsAbsoluteUri)
                {
                    if (IsContainedLangInPath(appResourceDictionary.Source.AbsolutePath))
                    {
                        app.Resources.MergedDictionaries.Remove(appResourceDictionary);
                    }

                    if (Theme.IsInstalled(app, appResourceDictionaries))
                    {
                        libraryResourceDictionary = appResourceDictionary;

                        for (int i = 0; i < appResourceDictionary.MergedDictionaries.Count; i++)
                        {
                            if (IsContainedLangInPath(appResourceDictionary.MergedDictionaries[i].Source.AbsolutePath))
                            {
                                appResourceDictionary.MergedDictionaries.Remove(appResourceDictionary.MergedDictionaries[i]);
                            }
                        }
                    }
                }
            }

            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(GetUriStringForLangInApp(Description)) });
            if (libraryResourceDictionary != null)
            {
                libraryResourceDictionary.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(GetUriStringForLangInLib(Description)) });
            }
        }
        #endregion Public

        #endregion Methods
    }
}
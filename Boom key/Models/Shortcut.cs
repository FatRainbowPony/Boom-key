using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using Newtonsoft.Json;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.Models
{
    public class Shortcut : ViewModel
    {
        #region Fields

        #region Private
        private readonly string uriStrToQuestionIcon = $"pack://application:,,,/{App.Name};component/Assets/Icons/QuestionIcon.ico";
        private string? name;
        private HotKey hotKey;
        private ImageSource icon;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string? Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public string CreationDate { get; set; }

        public string? PathToExetubaleObj { get; set; }

        public string? PathToIcon { get; set; }

        public HotKey HotKey
        {
            get => hotKey;
            set => Set(ref hotKey, value);
        }

        [JsonIgnore]
        public ImageSource Icon
        {
            get => icon;
            set => Set(ref icon, value);
        }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Shortcut()
        {
            CreationDate = DateTime.Today.ToString("G");
            Icon = new BitmapImage(new Uri(uriStrToQuestionIcon));
            HotKey = new HotKey();
        }

        public Shortcut(Shortcut shortcut)
        {
            shortcut ??= new Shortcut();
            Name = shortcut.Name;
            CreationDate = shortcut.CreationDate;
            PathToExetubaleObj = shortcut.PathToExetubaleObj;
            PathToIcon = shortcut.PathToIcon;
            Icon = shortcut.Icon.Clone();
            HotKey = new HotKey(shortcut.HotKey);
        }

        public Shortcut(string name, string pathToExetubaleObj)
        {
            Name = name;
            CreationDate = DateTime.Now.ToString("G");
            PathToExetubaleObj = pathToExetubaleObj;
            ImageSource? icon = GetIcon(pathToExetubaleObj);
            if (icon == null)
            {
                Icon = new BitmapImage(new Uri(uriStrToQuestionIcon));
            }
            else
            {
                Icon = icon;
                PathToIcon = pathToExetubaleObj;
            }
            HotKey = new HotKey();
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private static ImageSource? GetIcon(string pathToIcon)
        {
            ImageSource? icon = null;

            if (pathToIcon != null)
            {
                if (File.Exists(pathToIcon))
                {
                    try
                    {
                        icon = appDevToolsExts.File.GetIcon(pathToIcon);
                    }
                    catch { }
                }

                if (Directory.Exists(pathToIcon))
                {
                    try
                    {
                        icon = appDevToolsExts.Directory.GetIcon(pathToIcon);
                    }
                    catch { }
                }
            }

            return icon;
        }
        #endregion Private

        #region Public
        public void RestoreIcon()
        {
            ImageSource? icon = null;

            if (File.Exists(PathToIcon))
            {
                icon = appDevToolsExts.File.GetIcon(PathToIcon);
            }

            if (Directory.Exists(PathToIcon))
            {
                icon = appDevToolsExts.Directory.GetIcon(PathToIcon);
            }

            if (icon != null)
            {
                Icon = icon;
            }
            else
            {
                Icon = new BitmapImage(new Uri(uriStrToQuestionIcon));
                PathToIcon = null;
            }
        }

        public static void ShowErrorAboutWrongPathToExetubaleObj()
        {
            MessageBox.Show
            (
                $"{Application.Current.Resources["ErrorAboutExistingObjDescription"]}",
                App.Name,
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK
            );
        }

        public static void ShowErrorAboutFailedGettingIcon()
        {
            MessageBox.Show
            (
                $"{Application.Current.Resources["ErrorAboutFailedGettingIconForObj"]}",
                App.Name,
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK
            );
        }
        #endregion Public

        #endregion Methods
    }
}
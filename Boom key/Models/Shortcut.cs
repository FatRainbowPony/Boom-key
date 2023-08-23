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
    public abstract class Shortcut : ViewModel
    {
        #region Fields

        #region Private
        private readonly string uriStrToQuestionIcon;
        private string? name;
        private HotKey hotKey;
        private ImageSource? icon;
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

        public HotKey HotKey
        {
            get => hotKey;
            set => Set(ref hotKey, value);
        }

        [JsonIgnore]
        public ImageSource? Icon
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
            uriStrToQuestionIcon = $"pack://application:,,,/{App.Name};component/Assets/Icons/QuestionIcon.ico";
            CreationDate = DateTime.Today.ToString("F");
            HotKey = new HotKey();
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Protected
        protected static ImageSource? GetIcon(string pathToExetubaleObj)
        {
            ImageSource? icon = null;

            if (pathToExetubaleObj != null)
            {
                if (Directory.Exists(pathToExetubaleObj))
                {
                    try
                    {
                        icon = appDevToolsExts.Directory.GetIcon(pathToExetubaleObj);
                    }
                    catch { }
                }

                if (File.Exists(pathToExetubaleObj))
                {
                    try
                    {
                        icon = appDevToolsExts.File.GetIcon(pathToExetubaleObj);
                    }
                    catch { }
                }
            }

            return icon;
        }
        #endregion Protected

        #region Public
        public void RestoreIcon()
        {
            if (File.Exists(PathToExetubaleObj))
            {
                Icon = appDevToolsExts.File.GetIcon(PathToExetubaleObj);
            }

            if (Directory.Exists(PathToExetubaleObj))
            {
                Icon = appDevToolsExts.Directory.GetIcon(PathToExetubaleObj);
            }

            if (!File.Exists(PathToExetubaleObj) && !Directory.Exists(PathToExetubaleObj))
            {
                Icon = new BitmapImage(new Uri(uriStrToQuestionIcon));
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
        #endregion Public

        #endregion Methods
    }
}
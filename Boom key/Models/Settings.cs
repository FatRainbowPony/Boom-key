using System.Windows;
using System.Windows.Media;
using AppDevTools.Addons;

namespace BoomKey.Models
{
    public class Settings
    {
        #region Property

        #region Public
        public bool UseAutorun { get; set; }

        public double TimeoutBeforeAutorun { get; set; }

        public bool RunHidden { get; set; }

        public double TimeoutBeforeAutohide { get; set; }

        public bool UseTopmost { get; set; }

        public Theme Theme { get; set; }

        public bool UseSysTheme { get; set; }

        public SolidColorBrush TitleBarColor { get; set; }

        public Addons.Lang Lang { get; set; }
        #endregion Public

        #endregion Property

        #region Constructors

        #region Public
        public Settings()
        {
            Theme = new Theme();
            UseSysTheme = true;
            TitleBarColor = (SolidColorBrush)Application.Current.Resources["TitleBarBackgroundBrush"];
            Lang = new Addons.Lang();  
        }
        #endregion Public

        #endregion Constructors
    }
}
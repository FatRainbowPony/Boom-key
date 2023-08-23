using System.Windows.Media;

namespace AppDevTools.Windows.LoadingWindow.Models
{
    public class Settings
    {
        #region Constants

        #region Private
        private const string DEF_WINDOW_NAME = "LoadingWindow";
        private const string DEF_TITLE = "Title";
        private const string DEF_ANNOTATION = "Annonation";
        #endregion Private

        #endregion Constants

        #region Fields

        #region Private
        private static Color defElementColor = Color.FromArgb(255, 128, 128, 128);
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string WindowName { get; set; }

        public ImageSource? Icon { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public static Color DefElementColor
        {
            get { return defElementColor; }
        }

        public SolidColorBrush? ElementBrush { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Settings()
        {
            WindowName = DEF_WINDOW_NAME;
            Title = DEF_TITLE;
            Annotation = DEF_ANNOTATION;
            ElementBrush = new SolidColorBrush(DefElementColor);
        }

        public Settings(string windowName, ImageSource icon, string title, string annotation, SolidColorBrush elementBrush)
        {
            windowName ??= DEF_WINDOW_NAME;
            annotation ??= DEF_ANNOTATION;
            elementBrush ??= new SolidColorBrush(DefElementColor);

            WindowName = windowName;
            Icon = icon;
            Title = title;
            Annotation = annotation;
            ElementBrush = elementBrush;
        }
        #endregion Public

        #endregion Constructors
    }
}
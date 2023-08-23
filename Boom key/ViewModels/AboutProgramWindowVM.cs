using AppDevTools.Templates.MVVM.ViewModel.Base;

namespace BoomKey.ViewModels
{
    public class AboutProgramWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private string appName = App.Name;
        private string appVersion = App.Version;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string AppName
        {
            get => appName;
            set => Set(ref appName, value);
        }

        public string AppVersion
        {
            get => appVersion;
            set => Set(ref appVersion, value);
        }
        #endregion Public

        #endregion Properties
    }
}
using Notification.Wpf;

namespace AppUpdateInstaller.Models
{
    public class InstallerInfo
    {
        #region Fields

        #region Private
        private static readonly string installerName = "App update installer";
        private static readonly string nameSource = "source";
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static string InstallerName
        {
            get { return installerName; }
        }

        public static string NameSource
        {
            get { return nameSource; }
        }

        public string AppName { get; set; }

        public string PathToAppIcon { get; set; }

        public string PathToUpdate { get; set; }

        public NotificationContent ContentForNotification { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public 
        public InstallerInfo(string appName, string pathToAppIcon, string pathToUpdate, NotificationContent contentForNotification)
        {
            appName ??= string.Empty;
            pathToAppIcon ??= string.Empty;
            pathToUpdate ??= string.Empty;
            contentForNotification ??= new NotificationContent();

            AppName = appName;
            PathToAppIcon = pathToAppIcon;
            PathToUpdate = pathToUpdate;
            ContentForNotification = contentForNotification;
        }
        #endregion Public

        #endregion Constructors
    }
}
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;
using AppInstaller.Addons;
using SingleInstanceCore;
using appDevToolsExts = AppDevTools.Extensions;

namespace AppInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        #region Fields

        #region Private
        private static readonly string appNameToInstall = "Boom key";
        private static readonly string appNameVerToInstall = "1.2.0.0";
        private static readonly string pathToInstall = appDevToolsExts.Directory.GetPathToDirInAppData(Path.Combine(appNameToInstall, "source"), true)!;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static string AppNameToInstall
        {
            get { return appNameToInstall; }
        }

        public static string AppNameVerToInstall
        {
            get { return appNameVerToInstall; }
        }

        public static string PathToInstall
        {
            get { return pathToInstall; }
        }

        public static bool IsFirstInstance { get; private set; }

        public static string Name { get; private set; }

        public static Lang InstallerLang { get; set; }

        public static string PathToInstallerIcon { get; private set; }

        public static string PathToSourceInstallation { get; private set; }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method for startup application
        private void AppStartup(object sender, StartupEventArgs e)
        {
            Name = $"{GetType().Assembly.GetName().Name}";

            IsFirstInstance = this.InitializeAsFirstInstance(Name);
            if (!IsFirstInstance)
            {
                Current.Shutdown();
            }
            else
            {
                string pathToTempDir = appDevToolsExts.Directory.GetPathToDirInAppData("temp", true)!;
                Directory.Delete(pathToTempDir, true);
                Directory.CreateDirectory(pathToTempDir);

                string pathToZip = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Source"), $"{AppNameToInstall}.zip");
                ZipFile.ExtractToDirectory(pathToZip, pathToTempDir);

                string pathToSourceDirInTemp = Path.Combine(pathToTempDir, AppNameToInstall);
                appDevToolsExts.File.CopyFilesRecursively(pathToSourceDirInTemp, pathToTempDir);
                Directory.Delete(pathToSourceDirInTemp, true);

                PathToSourceInstallation = pathToTempDir;
                PathToInstallerIcon = $"pack://application:,,,/{Name};component/Assets/Icons/AppIcon.ico";
                InstallerLang = new Lang();
                InstallerLang.Set(Current, Current.Resources.MergedDictionaries.ToList());

                string? installDirName = Path.GetDirectoryName(pathToInstall);
                if (!string.IsNullOrEmpty(installDirName) && Directory.Exists(installDirName))
                {
                    Directory.Delete(installDirName, true);
                }
                Directory.CreateDirectory(pathToInstall);
            }
        }
        #endregion Method for startup application

        #region Method to exit from application
        private void AppExit(object sender, ExitEventArgs e)
        {
            SingleInstance.Cleanup();
        }
        #endregion Method to exit from application

        #region Method to get instance of the application already exists
        void ISingleInstance.OnInstanceInvoked(string[] args)
        {
            Process currentProcess = Process.GetCurrentProcess();
            appDevToolsExts.Window.Activate(currentProcess.MainWindowHandle);
            appDevToolsExts.Window.Show(currentProcess.MainWindowHandle, appDevToolsExts.Window.WindowStateWin32.SW_RESTORE);
        }
        #endregion Method to get instance of the application already exists

        #endregion Private

        #endregion Methods
    }
}
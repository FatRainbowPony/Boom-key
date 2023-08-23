using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using AppDevTools.Addons;
using AppUpdateInstaller.Models;
using SingleInstanceCore;
using appDevToolsExts = AppDevTools.Extensions;

namespace AppUpdateInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        #region Properties

        #region Public
        public static InstallerInfo? InstallerInfo { get; private set; }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method for startup application
        private void AppStartup(object sender, StartupEventArgs e)
        {
            string appName = $"{GetType().Assembly.GetName().Name}";

            bool isFirstInstance = this.InitializeAsFirstInstance(appName);
            if (!isFirstInstance)
            {
                Current.Shutdown();
            }
            else if (e.Args.Length > 0)
            {
                List<InstallerInfo>? infoAboutInstall = Json.GetDeserializedObj<InstallerInfo>(e.Args[0]);
                if (infoAboutInstall != null && infoAboutInstall.FirstOrDefault() != null)
                {
                    InstallerInfo = infoAboutInstall.FirstOrDefault();
                }
            }
        }

        void ISingleInstance.OnInstanceInvoked(string[] args)
        {
            Process currentProcess = Process.GetCurrentProcess();
            appDevToolsExts.Window.Activate(currentProcess.MainWindowHandle);
            appDevToolsExts.Window.Show(currentProcess.MainWindowHandle, appDevToolsExts.Window.WindowStateWin32.SW_RESTORE);
        }
        #endregion Method for startup application

        #endregion Private

        #endregion Methods
    }
}
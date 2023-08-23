using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using AppUpdateInstaller.Models;
using Notification.Wpf;
using Notification.Wpf.Classes;
using appDevToolsExts = AppDevTools.Extensions;

namespace AppUpdateInstaller.ViewModels
{
    public class MainWindowVM : ViewModel
    {
        #region Constants

        #region Private
        private const string DEF_WINDOWNAME = "MainWindow";
        #endregion Private

        #endregion Constants

        #region Fields

        #region Private
        private string windowName;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string WindowName
        {
            get => windowName;
            set => Set(ref windowName, value);
        }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public MainWindowVM()
        {
            windowName ??= DEF_WINDOWNAME;
        }
        #endregion Public

        #endregion Constructors

        #region Commands 

        #region Public

        #region Command for startup window
        public ICommand StartupWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (App.InstallerInfo != null)
                    {
                        string pathToUpdateDir = App.InstallerInfo.PathToUpdate;
                        string? pathToSourceDir = null;
                        string? pathToBaseDir = Path.GetDirectoryName(pathToUpdateDir);
                        if (!string.IsNullOrEmpty(pathToBaseDir))
                        {
                            pathToSourceDir = Path.Combine(pathToBaseDir, InstallerInfo.NameSource);
                        }
                        if (Directory.Exists(pathToSourceDir))
                        {
                            WindowName = App.InstallerInfo.ContentForNotification.Title;

                            NotificationManager notificationManager = new();
                            NotifierProgress<(double?, string, string, bool?)> notifierProgress = notificationManager.ShowProgressBar
                            (
                                App.InstallerInfo.ContentForNotification.Title,
                                false,
                                false,
                                "",
                                false,
                                1,
                                "",
                                false,
                                true,
                                App.InstallerInfo.ContentForNotification.Background,
                                App.InstallerInfo.ContentForNotification.Foreground,
                                null,
                                appDevToolsExts.File.GetIcon(App.InstallerInfo.PathToAppIcon),
                                null,
                                null,
                                true
                            );

                            DispatcherTimer? reportTimer = new() { Interval = TimeSpan.FromSeconds(1) };
                            reportTimer.Tick += (sender, e) => { notifierProgress.Report((null, App.InstallerInfo.ContentForNotification.Message, App.InstallerInfo.ContentForNotification.Title, false)); };
                            reportTimer.Start();

                            Directory.Delete(pathToSourceDir, true);
                            Directory.CreateDirectory(pathToSourceDir);
                            appDevToolsExts.File.CopyFilesRecursively(pathToUpdateDir, pathToSourceDir);

                            reportTimer.Stop();
                            reportTimer = null;

                            notifierProgress.Dispose();

                            Process.Start(new ProcessStartInfo { FileName = Path.Combine(pathToSourceDir, $"{App.InstallerInfo.AppName}.exe") });
                            Application.Current.Shutdown();
                        }
                    }                   
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #endregion Public

        #endregion Commands 
    }
}
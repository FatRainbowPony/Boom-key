using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AppDevTools.Addons.AppUpdateLoader;
using AppDevTools.Windows.LoadingWindow.ViewModels;
using AppDevTools.Windows.LoadingWindow.Views;
using AppUpdateInstaller.Models;
using BoomKey.Models;
using Notification.Wpf;
using SingleInstanceCore;
using appDevToolsAddns = AppDevTools.Addons;
using appDevToolsExts = AppDevTools.Extensions;
using appUpdateLoader = AppDevTools.Addons.AppUpdateLoader;
using loadingWin = AppDevTools.Windows.LoadingWindow;

namespace BoomKey
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        #region Constants

        #region Private
        private const string DEF_DEVELOPER_NICKNAME = "FatRainbowPony";
        private const string DEF_REPOSITORY_NAME = "Boom-key";
        private const string DEF_SOURCE_NAME = DEF_REPOSITORY_NAME;
        private const string DEF_DIRNAME_UPDATE = "update";
        private const string DEF_DIRNAME_INFO = "info";
        private const string DEF_FILENAME_INFO_SHORTCUTS = "shortcuts.info";
        private const string DEF_DIRNAME_INFO_WINDOWS = "windows";
        private const string DEF_FILENAME_INFO_MAIN_WINDOW = "main.info";
        private const string DEF_FILENAME_INFO_COLORS = "colors.info";
        private const string DEF_SETTINGS_NAME = "settings.config";
        #endregion Private

        #endregion Constants

        #region Properties

        #region Public
        public static string DeveloperNickname { get; private set; }

        public static string RepositoryName { get; private set; }

        public static string SourceName { get; private set; }

        public static string Name { get; private set; }

        public static string PathToDir { get; private set; }

        public static string Location { get; private set; }

        public static string Version { get; private set; }

        public static string PathToIconAsFile { get; private set; }

        public static string PathToIconAsResource { get; private set; }

        public static BitmapImage Icon { get; private set; }

        public static bool IsFirstInstance { get; private set; }
        
        public static string PathToWorkingDir { get; private set; }

        public static string PathToSourceDir { get; private set; }

        public static string PathToUpdateDir { get; private set; }

        public static string PathToInfoDir { get; private set; }

        public static string PathToInfoShortcuts { get; private set; }

        public static List<Section>? InfoShortcuts { get; private set; }

        public static string PathToInfoWindowsDir { get; private set; }

        public static string PathToInfoMainWindow { get; private set; }

        public static string PathToInfoColors { get; private set; }

        public static List<SolidColorBrush> PersonalizationColors { get; private set; }

        public static string PathToSettings { get; private set; }

        public static Settings Settings { get; set; }

        public static NotificationManager NotificationManager { get; private set; }

        public static UpdateLoader UpdateLoader { get; private set; }

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
                DeveloperNickname = DEF_DEVELOPER_NICKNAME;
                RepositoryName = DEF_REPOSITORY_NAME;
                SourceName = DEF_SOURCE_NAME;

                PathToDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
                Location = Path.Combine(PathToDir, $"{GetType().Assembly.GetName().Name}.exe");

                Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (assemblyVersion != null)
                {
                    Version = assemblyVersion.ToString();
                }

                PathToIconAsFile = $"{PathToDir}\\Assets\\Icons\\DefAppIcon.ico";
                PathToIconAsResource = $"pack://application:,,,/{Name};component/Assets/Icons/TrayAppIcon.ico";
                Icon = new BitmapImage(new Uri(PathToIconAsResource));

                PathToWorkingDir = appDevToolsExts.Directory.GetPathToDirInAppData(Name, true)!;

                PathToSettings = Path.Combine(PathToWorkingDir, DEF_SETTINGS_NAME);
                if (!File.Exists(PathToSettings))
                {
                    File.Create(PathToSettings).Close();
                }
                List<Settings>? contentSettings = appDevToolsExts.File.Load<Settings>(PathToSettings);
                if (contentSettings != null && contentSettings.FirstOrDefault() != null)
                {
                    Settings = contentSettings.First();
                }
                else
                {
                    Settings = new Settings();
                }
                appDevToolsExts.File.Save(PathToSettings, new List<Settings> { Settings });

                if (Settings.TimeoutBeforeAutorun > 0)
                {
                    Task.Delay(TimeSpan.FromMinutes(Settings.TimeoutBeforeAutorun)).Wait();
                }
                Settings.Theme.Set(Current, Current.Resources.MergedDictionaries.ToList());
                appDevToolsAddns.Colors.SetTitleBarColor(Current, Settings.TitleBarColor);
                Settings.Lang.Set(Current, Current.Resources.MergedDictionaries.ToList());

                Thread appLoadingThread = new(() => 
                {
                    LoadingWindowVM loadingWinContext = new(new loadingWin.Models.Settings($"{Current.Resources["LoadingAppDataWindowTitle"]}", new BitmapImage(new Uri(PathToIconAsResource)), Name, $"{Current.Resources["LoadingAppDataDescription"]}", (SolidColorBrush)Current.Resources["LoadingElementBrush"]));
                    LoadingWindow loadingWindow = new() { DataContext = loadingWinContext };
                    AppLoadingIsOver += () =>
                    {
                        loadingWinContext.StopAnimation();
                        Dispatcher.FromThread(loadingWinContext.OwnerThread).Invoke(() => loadingWindow?.Close());
                    };
                    loadingWindow.ShowDialog();
                });
                appLoadingThread.SetApartmentState(ApartmentState.STA);
                appLoadingThread.Start();

                PathToSourceDir = Path.Combine(PathToWorkingDir, InstallerInfo.NameSource);
                if (!Directory.Exists(PathToSourceDir))
                {
                    Directory.CreateDirectory(PathToSourceDir);
                }

                PathToUpdateDir = Path.Combine(PathToWorkingDir, DEF_DIRNAME_UPDATE);
                if (Directory.Exists(PathToUpdateDir))
                {
                    Directory.Delete(PathToUpdateDir, true);
                }
                Directory.CreateDirectory(PathToUpdateDir);

                PathToInfoDir = Path.Combine(PathToWorkingDir, DEF_DIRNAME_INFO);
                if (!Directory.Exists(PathToInfoDir))
                {
                    Directory.CreateDirectory(PathToInfoDir);
                }

                PathToInfoShortcuts = Path.Combine(PathToInfoDir, DEF_FILENAME_INFO_SHORTCUTS);
                if (!File.Exists(PathToInfoShortcuts))
                {
                    File.Create(PathToInfoShortcuts).Close();
                }
                List<Section>? sections = appDevToolsExts.File.Load<Section>(PathToInfoShortcuts);
                if (sections != null)
                {
                    InfoShortcuts = sections;

                    foreach (Section section in sections)
                    {
                        if (section.Shortcuts != null)
                        {                            
                            foreach (Shortcut shortcut in section.Shortcuts)
                            {
                                shortcut.RestoreIcon();
                            }
                        }
                    }
                }

                PathToInfoWindowsDir = Path.Combine(PathToInfoDir, DEF_DIRNAME_INFO_WINDOWS);
                if (!Directory.Exists(PathToInfoWindowsDir))
                {
                    Directory.CreateDirectory(PathToInfoWindowsDir);
                }

                PathToInfoMainWindow = Path.Combine(PathToInfoWindowsDir, DEF_FILENAME_INFO_MAIN_WINDOW);
                if (!File.Exists(PathToInfoMainWindow))
                {
                    File.Create(PathToInfoMainWindow).Close();
                }

                PathToInfoColors = Path.Combine(PathToInfoDir, DEF_FILENAME_INFO_COLORS);
                if (!File.Exists(PathToInfoColors))
                {
                    File.Create(PathToInfoColors).Close();
                }
                List<SolidColorBrush>? contentInfoColors = appDevToolsExts.File.Load<SolidColorBrush>(PathToInfoColors);
                if (contentInfoColors != null && contentInfoColors.FirstOrDefault() != null)
                {
                    PersonalizationColors = contentInfoColors;
                }
                else
                {
                    PersonalizationColors = (from color in Addons.Colors.Get() select new SolidColorBrush(color)).ToList();
                }
                appDevToolsExts.File.Save(PathToInfoColors, PersonalizationColors);

                NotificationManager = new NotificationManager();
                UpdateLoader = new(new appUpdateLoader.Models.Settings(Version, DeveloperNickname, RepositoryName, SourceName, Path.Combine(PathToUpdateDir, $"update.zip")));

                Task.Delay(2000).Wait();

                AppLoadingIsOver?.Invoke();
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

        #region Events

        #region Private
        private event Action? AppLoadingIsOver;
        #endregion Private

        #endregion Events
    }
}
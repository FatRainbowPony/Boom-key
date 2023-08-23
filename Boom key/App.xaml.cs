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
using AppDevTools.Addons;
using AppDevTools.Windows.LoadingWindow.ViewModels;
using AppDevTools.Windows.LoadingWindow.Views;
using BoomKey.Models;
using SingleInstanceCore;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        #region Constants

        #region Private
        private const string DEF_DIRNAME_INFO = "info";
        private const string DEF_FILENAME_INFO_SHORTCUTS = "shortcuts.info";
        private const string DEF_DIRNAME_INFO_WINDOWS = "windows";
        private const string DEF_FILENAME_INFO_MAIN_WINDOW = "main.info";
        private const string DEF_FILENAME_INFO_SUB_WINDOW = "sub.info";
        private const string DEF_FILENAME_INFO_COLORS = "colors.info";
        //private const string DEF_SETTINGS_NAME = "settings.config";
        #endregion Private

        #endregion Constants

        #region Fields

        #region Private
        private string uriStrToAppIcon;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static string Name { get; private set; }

        public static string Location { get; private set; }

        public static string Version { get; private set; }

        public static bool IsFirstInstance { get; private set; }
        
        public static string PathToWorkingDir { get; private set; }

        public static string PathToInfoDir { get; private set; }

        public static string PathToInfoShortcuts { get; private set; }

        public static ShortcutsInfo? ShortcutsInfo { get; private set; }

        public static string PathToInfoWindowsDir { get; private set; }

        public static string PathToInfoMainWindow { get; private set; }

        public static string PathToInfoSubWindow { get; private set; }

        public static string PathToInfoColors { get; private set; }

        public static List<SolidColorBrush> PersonalizationColors { get; private set; }

        //public static string PathToSettings { get; private set; }

        //public static Settings Settings { get; set; }
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
                //PathToSettings = Path.Combine(PathToWorkingDir, DEF_SETTINGS_NAME);
                //if (!File.Exists(PathToSettings))
                //{
                //    File.Create(PathToSettings).Close();
                //}
                //List<Settings>? contentSettings = appDevToolsExts.File.Load<Settings>(PathToSettings);
                //if (contentSettings != null && contentSettings.FirstOrDefault() != null)
                //{
                //    Settings = new Settings(contentSettings.First());
                //}
                //else
                //{
                //    Settings = new Settings();
                //}
                //appDevToolsExts.File.Save(PathToSettings, new List<Settings> { Settings });             

                Location = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)}\\{GetType().Assembly.GetName().Name}.exe";
                Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (assemblyVersion != null)
                {
                    Version = assemblyVersion.ToString();
                }

                uriStrToAppIcon = $"pack://application:,,,/{Name};component/AppIcon.ico";

                Theme sysTheme = Theme.Get();
                if (!sysTheme.IsEqual(Current, Current.Resources.MergedDictionaries.ToList()))
                {
                    sysTheme.Set(Current, Current.Resources.MergedDictionaries.ToList());
                }

                Autorun autorun = new();
                object? autorunRegistry = autorun.Get(Name);
                if (autorunRegistry != null)
                {
                    autorun.Delete(Name);
                }
                autorun.Set(Name, Location);

                Thread loadingThread = new(() => 
                {
                    LoadingWindowVM loadingWinContext = new($"{Current.Resources["LoadingWindowTitle"]}", new BitmapImage(new Uri(uriStrToAppIcon)), Name, $"{Current.Resources["LoadingDescription"]}");
                    LoadingWindow loadingWindow = new() { Topmost = true, DataContext = loadingWinContext };
                    LoadingIsOver += () =>
                    {
                        loadingWinContext.StopAnimation();
                        Dispatcher.FromThread(loadingWinContext.OwnerThread).Invoke(() => loadingWindow?.Close());
                    };
                    loadingWindow.ShowDialog();
                });
                loadingThread.SetApartmentState(ApartmentState.STA);
                loadingThread.Start();

                PathToWorkingDir = appDevToolsExts.Directory.GetPathToDirInAppData(Name, true)!;

                PathToInfoDir = Path.Combine(PathToWorkingDir, DEF_DIRNAME_INFO);
                if (!Directory.Exists(PathToInfoDir))
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(PathToInfoDir);
                    directoryInfo.Attributes = FileAttributes.Hidden;
                }

                PathToInfoShortcuts = Path.Combine(PathToInfoDir, DEF_FILENAME_INFO_SHORTCUTS);
                if (!File.Exists(PathToInfoShortcuts))
                {
                    File.Create(PathToInfoShortcuts).Close();
                }
                List<ShortcutsInfo>? contentShortcutsInfo = appDevToolsExts.File.Load<ShortcutsInfo>(PathToInfoShortcuts);
                if (contentShortcutsInfo != null && contentShortcutsInfo.FirstOrDefault() != null)
                {
                    ShortcutsInfo = contentShortcutsInfo.First();

                    if (ShortcutsInfo.FavoriteShortcuts != null || ShortcutsInfo.ShortcutSections != null)
                    {
                        if (ShortcutsInfo.FavoriteShortcuts != null)
                        {
                            foreach (FavoriteShortcut favoriteShortcut in ShortcutsInfo.FavoriteShortcuts)
                            {
                                favoriteShortcut.RestoreIcon();
                            }
                        }

                        if (ShortcutsInfo.ShortcutSections != null)
                        {
                            foreach (Section shortcutSection in ShortcutsInfo.ShortcutSections)
                            {
                                if (shortcutSection.Shortcuts != null)
                                {
                                    foreach (NormalShortcut shortcut in shortcutSection.Shortcuts)
                                    {
                                        shortcut.RestoreIcon();
                                    }
                                }
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

                PathToInfoSubWindow = Path.Combine(PathToInfoWindowsDir, DEF_FILENAME_INFO_SUB_WINDOW);
                if (!File.Exists(PathToInfoSubWindow))
                {
                    File.Create(PathToInfoSubWindow).Close();
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
                    PersonalizationColors = (from color in Addons.Colors.GetAll() select new SolidColorBrush(color)).ToList();
                }
                appDevToolsExts.File.Save(PathToInfoColors, PersonalizationColors);

                Task.Delay(2000).Wait();

                LoadingIsOver?.Invoke();
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
        private event Action? LoadingIsOver;
        #endregion Private

        #endregion Events
    }
}
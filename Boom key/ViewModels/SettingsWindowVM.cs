using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using BoomKey.Addons;
using BoomKey.Models;
using BoomKey.Views;
using Notification.Wpf;
using Notification.Wpf.Classes;
using appDevToolsAddns = AppDevTools.Addons;
using appDevToolsExts = AppDevTools.Extensions;
using appUpdateLoader = AppDevTools.Addons.AppUpdateLoader;

namespace BoomKey.ViewModels
{
    public class SettingsWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private bool enabledSettings;
        private bool useAutorun;
        private double timeoutBeforeAutorun;
        private bool runHidden;
        private double timeoutBeforeAutohide;
        private bool useTopmost;
		private bool useLightTheme;
		private bool useDarkTheme;
		private bool useSysTheme;
		private SolidColorBrush? currentTitleBarColor;
        private ObservableCollection<string> langs = new(Enum.GetNames(typeof(Lang.LangDescription)).ToList());
		private string? selectedLang;
        private string? appVersion;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public bool EnabledSettings
        {
            get => enabledSettings;
            set => Set(ref enabledSettings, value);
        }

        public bool UseAutorun
        {
            get => useAutorun;
            set => Set(ref useAutorun, value);
        }

        public double TimeoutBeforeAutorun
        {
            get => timeoutBeforeAutorun;
            set => Set(ref timeoutBeforeAutorun, value);
        }

        public bool RunHidden
        {
            get => runHidden;
            set => Set(ref runHidden, value);
        }

        public double TimeoutBeforeAutohide
        {
            get => timeoutBeforeAutohide;
            set => Set(ref timeoutBeforeAutohide, value);
        }

        public bool UseTopmost
        {
            get => useTopmost;
            set => Set(ref useTopmost, value);
        }

        public bool UseLightTheme
        {
			get => useLightTheme;
			set => Set(ref useLightTheme, value);
		}

		public bool UseDarkTheme
        {
			get => useDarkTheme;
			set => Set(ref useDarkTheme, value);
		}

		public bool UseSysTheme
        {
			get => useSysTheme;
			set => Set(ref useSysTheme, value);
		}

		public SolidColorBrush? CurrentTitleBarColor
        {
			get => currentTitleBarColor;
			set => Set(ref currentTitleBarColor, value);
		}

		public ObservableCollection<string> Langs
        {
			get => langs;
			set => Set(ref langs, value);
		}

		public string? SelectedLang
        {
			get => selectedLang;
			set => Set(ref selectedLang, value);
		}

        public string? AppVersion
        {
            get => appVersion;
            set => Set(ref appVersion, value);
        }
        #endregion Public

        #endregion Properties

        #region Commands 

        #region Public

        #region Command for startup window
        public ICommand StartupWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    EnabledSettings = true;
                    UseAutorun = App.Settings.UseAutorun;
                    TimeoutBeforeAutorun = App.Settings.TimeoutBeforeAutorun;
                    RunHidden = App.Settings.RunHidden;
                    TimeoutBeforeAutohide = App.Settings.TimeoutBeforeAutohide;
                    UseTopmost = App.Settings.UseTopmost;
                    UseSysTheme = App.Settings.UseSysTheme;
                    if (useSysTheme)
                    {
                        UseLightTheme = false;
                        UseDarkTheme = false;
                    }
                    else
                    {
                        switch (App.Settings.Theme.Description)
                        {
                            case appDevToolsAddns.Theme.ThemeDescription.LightTheme:
                                UseLightTheme = true;
                                break;

                            case appDevToolsAddns.Theme.ThemeDescription.DarkTheme:
                                UseLightTheme = true;
                                break;
                        }
                    }
                    CurrentTitleBarColor = App.Settings.TitleBarColor;
                    SelectedLang = Enum.GetName(typeof(Lang.LangDescription), App.Settings.Lang.Description);
                    AppVersion = App.Version;
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #region Commands for changing autorun settings

        #region Command to set autorun
        public ICommand SetAutotunComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    appDevToolsAddns.Autorun autorun = new();
                    object? autorunRegistry = autorun.Get(App.Name);
                    if (autorunRegistry != null)
                    {
                        autorun.Delete(App.Name);
                    }
                    autorun.Set(App.Name, App.Location);
                    App.Settings.UseAutorun = true;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set autorun

        #region Command to delete autorun
        public ICommand DelAutorunComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    appDevToolsAddns.Autorun autorun = new();
                    object? autorunRegistry = autorun.Get(App.Name);
                    if (autorunRegistry != null)
                    {
                        autorun.Delete(App.Name);
                        App.Settings.UseAutorun = false;
                        appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                    }
                }, (obj) => true);
            }
        }
        #endregion Command to delete autorun

        #endregion Commands for changing autorun settings

        #region Command to set timeout before autorun
        public ICommand SetTimeoutBeforeAutorunComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    App.Settings.TimeoutBeforeAutorun = timeoutBeforeAutorun;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set timeout before autorun

        #region Сommands for changing the hidden startup settings

        #region Command to set run hiiden
        public ICommand SetRunHiddenComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    App.Settings.RunHidden = true;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set run hiiden

        #region Command to delete run hidden
        public ICommand DelRunHiddenComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    App.Settings.RunHidden = false;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to delete run hidden

        #endregion Сommands for changing the hidden startup settings

        #region Command to set timeout before autohide
        public ICommand SetTimeoutBeforeAutohideComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    App.Settings.TimeoutBeforeAutohide = timeoutBeforeAutohide;
                    if (timeoutBeforeAutohide > 0)
                    {
                        ChangedAutohide?.Invoke(true);
                    }
                    else
                    {
                        ChangedAutohide?.Invoke(false);
                    }
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set timeout before autohide

        #region Commands for changing topmost settings

        #region Command to set topmost
        public ICommand SetTopmostComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ChangedTopmost?.Invoke(true);
                    App.Settings.UseTopmost = true;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set topmost

        #region Command to delete topmost
        public ICommand DelTopmostComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ChangedTopmost?.Invoke(false);
                    App.Settings.UseTopmost = false;
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to delete topmost

        #endregion Commands for changing topmost settings

        #region Commands for changing theme

        #region Command to set light theme
        public ICommand SetLightThemeComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    UseSysTheme = false;
                    UseDarkTheme = false;
                    UseLightTheme = true;

                    App.Settings.UseSysTheme = false;
                    App.Settings.Theme = new appDevToolsAddns.Theme(appDevToolsAddns.Theme.ThemeDescription.LightTheme);
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set light theme

        #region Command to set dark theme
        public ICommand SetDarkThemeComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    UseSysTheme = false;
                    UseLightTheme = false;
                    UseDarkTheme = true;

                    App.Settings.UseSysTheme = false;
                    App.Settings.Theme = new appDevToolsAddns.Theme(appDevToolsAddns.Theme.ThemeDescription.DarkTheme);
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set dark theme

        #region Command to set system theme
        public ICommand SetSysThemeComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    UseLightTheme = false;
                    UseDarkTheme = false;

                    appDevToolsAddns.Theme sysTheme = appDevToolsAddns.Theme.Get();
                    if (sysTheme != null)
                    {
                        UseSysTheme = true;
                        App.Settings.UseSysTheme = true;
                        App.Settings.Theme = sysTheme;
                    }
                    appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                }, (obj) => true);
            }
        }
        #endregion Command to set system theme

        #endregion Commands for changing theme

        #region Command for changing title bar color
        public ICommand ChangeTitleBarColorComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    SolidColorBrush titleBar = currentTitleBarColor!;
                    ColorSelectorWindowVM colorSelectorWinContext = new()
                    {
                        Title = $"{Application.Current.Resources["ChangingColorWindowTitle"]} {((string)Application.Current.Resources["TitleBarDescription"]).ToLower()}",
                        OriginalColor = titleBar.Color
                    };
                    colorSelectorWinContext.ColorSelected += (color) =>
                    {
                        CurrentTitleBarColor = color;
                        appDevToolsAddns.Colors.SetTitleBarColor(Application.Current, color);
                        App.Settings.TitleBarColor = color;
                        appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                    };
                    new ColorSelectorWindow
                    {
                        DataContext = colorSelectorWinContext,
                        Owner = appDevToolsExts.Window.Get(Application.Current, $"{Application.Current.Resources["SettingsWindowTitle"]}")
                    }.ShowDialog();
                }, (obj) => true);
            }
        }
        #endregion Command for changing title bar color

        #region Command for changing language
        public ICommand ChangeLangComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is string langAsStr && langAsStr != null)
                    {
                        bool isSuccessfullyParse = Enum.TryParse(langAsStr, out Lang.LangDescription description);
                        if (isSuccessfullyParse)
                        {
                            Lang lang = new(description);
                            lang.Set(Application.Current, Application.Current.Resources.MergedDictionaries.ToList());
                            App.Settings.Lang = lang;
                            appDevToolsExts.File.Save(App.PathToSettings, new List<Settings> { App.Settings });
                        }
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for changing language

        #region Command to check update
        public ICommand CheckUpdateComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    App.UpdateLoader.StopSearchingUpdate();
                    App.UpdateLoader.CancelDownloadUpdate();

                    EnabledSettings = false;
                    bool hasUpdate = false;

                    NotifierProgress<(double?, string, string, bool?)> notifierProgress = App.NotificationManager.ShowProgressBar
                    (
                        $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                        false,
                        false,
                        "",
                        false,
                        1,
                        "",
                        false,
                        true,
                        (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                        (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"],
                        null,
                        App.Icon,
                        null,
                        null,
                        true
                    );

                    DispatcherTimer? notifierProgressTimer = new() { Interval = TimeSpan.FromSeconds(1) };
                    notifierProgressTimer.Tick += (sender, e) => 
                    { 
                        notifierProgress.Report
                        ((
                            null, 
                            $"{Application.Current.Resources["CheckingUpdateDescription"]}", 
                            $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}", 
                            false
                        )); 
                    };
                    notifierProgressTimer.Start();

                    App.UpdateLoader.UpdateAppeared += GetUpdate;
                    App.UpdateLoader.UpdateSearchIsOver += StopSearchingUpdate;
                    App.UpdateLoader.SearchUpdate();


                    void GetUpdate(appUpdateLoader.Models.Update update)
                    {
                        hasUpdate = true;
                    }

                    void StopSearchingUpdate()
                    {
                        App.UpdateLoader.StopSearchingUpdate();

                        notifierProgressTimer?.Stop();
                        notifierProgressTimer = null;
                        notifierProgress?.Dispose();

                        EnabledSettings = true;

                        if (!hasUpdate)
                        {
                            App.NotificationManager.Show(new NotificationContent
                            {
                                Title = $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                                Message = $"{Application.Current.Resources["ErrorCheckingUpdateDescription"]}",
                                Background = (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                                Foreground = (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"],
                                Icon = App.Icon,
                                CloseOnClick = true
                            });
                        }

                        App.UpdateLoader.UpdateAppeared -= GetUpdate;
                        App.UpdateLoader.UpdateSearchIsOver -= StopSearchingUpdate;
                    }
                }, (obj) => true);

            }
        }
        #endregion Command to check update

        #endregion Public

        #endregion Commands

        #region Events

        #region Public
        public event Action<bool>? ChangedAutohide;
        public event Action<bool>? ChangedTopmost;
        #endregion Public

        #endregion Events
    }
}
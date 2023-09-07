using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using AppInstaller.Addons;
using appDevToolsAddns = AppDevTools.Addons;
using appDevToolsExts = AppDevTools.Extensions;

namespace AppInstaller.ViewModels
{
    public class MainWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private string windowName;
        private ImageSource windowIcon;
        private BackgroundWorker inspectorInstallerTheme;
        private bool showingNullStep = true;
        private ObservableCollection<string> langs = new(Enum.GetNames(typeof(Lang.LangDescription)).ToList());
        private string? selectedLang;
        private bool showingFirstStep;
        private bool createDesktopShortcut;
        private bool showingSecondStep;
        private bool showingThirdStep;
        private bool showingFinalStep;
        private BackgroundWorker installer;
        private int installationProgress;
        private bool launchApp;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string WindowName
        {
            get => windowName;
            set => Set(ref windowName, value);
        }

        public ImageSource WindowIcon
        {
            get => windowIcon;
            set => Set(ref windowIcon, value);
        }

        public bool ShowingNullStep
        {
            get => showingNullStep;
            set => Set(ref showingNullStep, value);
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

        public bool ShowingFirstStep
        {
            get => showingFirstStep;
            set => Set(ref showingFirstStep, value);
        }

        public bool CreateDesktopShortcut
        {
            get => createDesktopShortcut;
            set => Set(ref createDesktopShortcut, value);
        }

        public bool ShowingSecondStep
        {
            get => showingSecondStep;
            set => Set(ref showingSecondStep, value);
        }

        public bool ShowingThirdStep
        {
            get => showingThirdStep;
            set => Set(ref showingThirdStep, value);
        }

        public int InstallationProgress
        {
            get => installationProgress;
            set => Set(ref installationProgress, value);
        }

        public bool ShowingFinalStep
        {
            get => showingFinalStep;
            set => Set(ref showingFinalStep, value);
        }

        public bool LaunchApp
        {
            get => launchApp;
            set => Set(ref launchApp, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void StartAppThemeInspector()
        {
            inspectorInstallerTheme = new() { WorkerSupportsCancellation = true };
            inspectorInstallerTheme.DoWork += (obj, e) =>
            {
                if (obj == null || obj is not BackgroundWorker)
                {
                    return;
                }

                BackgroundWorker inspectorInstallerTheme = (BackgroundWorker)obj;
                if (inspectorInstallerTheme.WorkerSupportsCancellation && inspectorInstallerTheme.CancellationPending)
                {
                    e.Cancel = true;

                    return;
                }

                appDevToolsAddns.Theme sysTheme = appDevToolsAddns.Theme.Get();
                if (!sysTheme.IsEqual(Application.Current, Application.Current.Resources.MergedDictionaries.ToList()))
                {
                    sysTheme.Set(Application.Current, Application.Current.Resources.MergedDictionaries.ToList());
                }
            };
            inspectorInstallerTheme.RunWorkerCompleted += (obj, e) =>
            {
                if (e.Cancelled)
                {
                    return;
                }

                inspectorInstallerTheme.RunWorkerAsync();
            };
            inspectorInstallerTheme.RunWorkerAsync();
        }

        private void StartInstallation(object? sender, DoWorkEventArgs e)
        {
            if (sender == null || sender is not BackgroundWorker)
            {
                return;
            }

            BackgroundWorker installer = (BackgroundWorker)sender;
            if (installer.WorkerSupportsCancellation && installer.CancellationPending)
            {
                e.Cancel = true;

                return;
            }

            if (e.Argument is List<string> pathsToFiles && pathsToFiles != null)
            {
                double step = pathsToFiles.Count * 0.01;

                List<string> pathsToDirs = Directory.GetDirectories(App.PathToSourceInstallation, "*", SearchOption.AllDirectories).ToList();
                for (int i = 0; i < pathsToDirs.Count(); i++)
                {
                    Directory.CreateDirectory(pathsToDirs[i].Replace(App.PathToSourceInstallation, App.PathToInstall));
                }

                for (int i = 0; i < pathsToFiles.Count; i++)
                {                    
                    File.Copy(pathsToFiles[i], pathsToFiles[i].Replace(App.PathToSourceInstallation, App.PathToInstall), true);
                    
                    if (installer.WorkerSupportsCancellation && installer.CancellationPending)
                    {
                        return;
                    }

                    if (installer.WorkerReportsProgress)
                    {
                        installer.ReportProgress(Convert.ToInt32(i / step));
                    }
                }

                if (installer.WorkerReportsProgress)
                {
                    installer.ReportProgress(100);
                }
            }
        }

        private void InstallationProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            InstallationProgress = e.ProgressPercentage;
        }

        private void InstallationCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (sender == null || sender is not BackgroundWorker || e.Cancelled)
            {
                return;
            }

            if (CreateDesktopShortcut)
            {
                string pathToShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{App.AppNameToInstall}.lnk");
                if (File.Exists(pathToShortcut))
                {
                    File.Delete(pathToShortcut);
                }
                appDevToolsExts.File.CreateShortcut(pathToShortcut, Path.Combine(App.PathToInstall, $"{App.AppNameToInstall}.exe"));
            }

            ShowingNullStep = false;
            ShowingFirstStep = false;
            ShowingSecondStep = false;
            ShowingThirdStep = false;
            ShowingFinalStep = true;
        }

        private void CancelInstallation()
        {
            installer.CancelAsync();
            installer.DoWork -= StartInstallation;
            installer.ProgressChanged -= InstallationProgressChanged;
            installer.RunWorkerCompleted -= InstallationCompleted;
            installer.Dispose();

            Application.Current.Shutdown();
        }
        #endregion Private

        #endregion Methods

        #region Commands 

        #region Command for startup window
        public ICommand StartupWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (App.IsFirstInstance)
                    {
                        WindowName = $"{Application.Current.Resources["AppInstallerTitle"]} - {App.AppNameToInstall} {App.AppNameVerToInstall}";
                        WindowIcon = new BitmapImage(new Uri(App.PathToInstallerIcon));
                        
                        StartAppThemeInspector();
                        
                        installer = new BackgroundWorker()
                        {
                            WorkerReportsProgress = true,
                            WorkerSupportsCancellation = true
                        };
                        installer.DoWork += StartInstallation;
                        installer.ProgressChanged += InstallationProgressChanged;
                        installer.RunWorkerCompleted += InstallationCompleted;
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #region Command for closing window
        public ICommand ClosingWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (showingFirstStep || showingSecondStep || showingThirdStep)
                    {
                        CancelInstallationComm.Execute(null);
                    }

                    inspectorInstallerTheme.CancelAsync();
                }, (obj) => true);
            }
        }
        #endregion Command for closing window

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
                            App.InstallerLang = new(description);
                            App.InstallerLang.Set(Application.Current, Application.Current.Resources.MergedDictionaries.ToList());
                        }
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for changing language

        #region Command for moving to first step of installation
        public ICommand MoveToFirstStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = false;
                    ShowingFirstStep = true;
                    ShowingSecondStep = false;
                    ShowingThirdStep = false;
                }, (obj) => true);
            }
        }
        #endregion Command for moving to first step of installation

        #region Command for moving to second step of installation
        public ICommand MoveToSecondStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = false;
                    ShowingFirstStep = false;
                    ShowingSecondStep = true;
                    ShowingThirdStep = false;
                }, (obj) => true);
            }
        }
        #endregion Command for moving to second step of installation

        #region Command for moving to third step of installation
        public ICommand MoveToThirdStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = false;
                    ShowingFirstStep = false;
                    ShowingSecondStep = false;
                    ShowingThirdStep = true;
                    installer.RunWorkerAsync(Directory.GetFiles(App.PathToSourceInstallation, "*.*", SearchOption.AllDirectories).ToList());
                }, (obj) => true);
            }
        }
        #endregion Command for moving to third step of installation

        #region Command to move back bull step of installation
        public ICommand MoveBackToNullStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = true;
                    ShowingFirstStep = false;
                    ShowingSecondStep = false;
                    ShowingThirdStep = false;
                }, (obj) => true);
            }
        }
        #endregion Command to move back bull step of installation

        #region Command to move back first step of installation
        public ICommand MoveBackToFirstStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = false;
                    ShowingFirstStep = true;
                    ShowingSecondStep = false;
                    ShowingThirdStep = false;
                }, (obj) => true);
            }
        }
        #endregion Command to move back first step of installation

        #region Command to move back second step of installation
        public ICommand MoveBackToSecondStepComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ShowingNullStep = false;
                    ShowingFirstStep = false;
                    ShowingSecondStep = true;
                    ShowingThirdStep = false;
                }, (obj) => true);
            }
        }
        #endregion Command to move back second step of installation

        #region Command to cancel installation
        public ICommand CancelInstallationComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    MessageBoxResult boxResult = MessageBox.Show
                    (
                        $"{Application.Current.Resources["QuestionAboutCancelInstallationDescription"]}",
                        windowName,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No
                    );
                    if (boxResult == MessageBoxResult.Yes)
                    {
                        CancelInstallation();
                    }
                }, (obj) => true);
            }
        }
        #endregion Command to cancel installation

        #region Command to finish installation
        public ICommand FinishInstallationComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (launchApp)
                    {
                        Process.Start(new ProcessStartInfo() { FileName = Path.Combine(App.PathToInstall, $"{App.AppNameToInstall}.exe") });
                    }

                    Application.Current.Shutdown();
                }, (obj) => true);
            }
        }
        #endregion Command to finish installation

        #endregion Commands
    }
}
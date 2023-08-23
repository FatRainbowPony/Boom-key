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
using AppDevTools.Addons;
using AppDevTools.Generators;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using AppUpdateInstaller.Models;
using BoomKey.Models;
using BoomKey.Views;
using GongSolutions.Wpf.DragDrop;
using NHotkey;
using NHotkey.Wpf;
using Notification.Wpf;
using WindowsDesktop;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class MainWindowVM : ViewModel, IDropTarget
    {
        #region Fields

        #region Private
        private Application currentApplication = Application.Current;
        private string appName = App.Name;
        private string pathToWindowInfo = App.PathToInfoMainWindow;
        private BackgroundWorker inspectorAboutAppTheme;
        private ObservableCollection<Section> sections = new();
        private Section? selectedSection;
        private Shortcut? selectedShortcut;
        private readonly List<string> pathsToDroppingObjs = new();
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public Application CurrentApplication
        {
            get => currentApplication;
            set => Set(ref currentApplication, value);
        }

        public string AppName
        {
            get => appName;
            set => Set(ref appName, value);
        }

        public string PathToWindowInfo
        {
            get => pathToWindowInfo;
            set => Set(ref pathToWindowInfo, value);
        }

        public ObservableCollection<Section> Sections
        {
            get => sections;
            set => Set(ref sections, value);
        }

        public Section? SelectedSection
        {
            get => selectedSection;
            set => Set(ref selectedSection, value);
        }

        public Shortcut? SelectedShortcut
        {
            get => selectedShortcut;
            set => Set(ref selectedShortcut, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method to restore info about shortcuts 
        private void RestoreInfoAboutShortcuts()
        {
            if (App.InfoShortcuts == null)
            {
                return;
            }

            foreach (Section section in App.InfoShortcuts)
            {
                Sections.Add(section);

                if (section.Shortcuts != null)
                {
                    foreach (Shortcut shortcut in section.Shortcuts)
                    {
                        if (shortcut.HotKey.Combination.MultModifierKey != Addons.MultModifierKey.None)
                        {
                            HotkeyManager.Current.AddOrReplace(shortcut.HotKey.Name, (Key)shortcut.HotKey.Combination.Key, (ModifierKeys)shortcut.HotKey.Combination.MultModifierKey, ExecuteShortcutByHotKey);
                        }
                    }
                }
            }
        }
        #endregion Method to restore info about shortcut

        #region Method to save info about shortcuts
        private void SaveInfoAboutShortcuts()
        {
            appDevToolsExts.File.Save(App.PathToInfoShortcuts, sections);
        }
        #endregion Method to save info about shorcuts

        #region Method to start the application theme inspector
        private void StartAppThemeInspector()
        {
            inspectorAboutAppTheme = new() { WorkerSupportsCancellation = true };
            inspectorAboutAppTheme.DoWork += (obj, e) =>
            {
                if (inspectorAboutAppTheme.CancellationPending)
                {
                    e.Cancel = true;

                    return;
                }

                Theme sysTheme = Theme.Get();
                if (!sysTheme.IsEqual(Application.Current, Application.Current.Resources.MergedDictionaries.ToList()))
                {
                    sysTheme.Set(Application.Current, Application.Current.Resources.MergedDictionaries.ToList());
                }
            };
            inspectorAboutAppTheme.RunWorkerCompleted += (obj, e) =>
            {
                if (e.Cancelled)
                {
                    return;
                }

                inspectorAboutAppTheme.RunWorkerAsync();
            };
            inspectorAboutAppTheme.RunWorkerAsync();
        }
        #endregion Method to start the application theme inspector

        #region Method to start the application update inspector
        private void StartAppUpdateInspector()
        {
            App.UpdateLoader.UpdateAppeared += (update) =>
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                    Message = $"{Application.Current.Resources["UpdateDescription"]} {update.Version}",
                    Background = (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                    Foreground = (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"],
                    Icon = App.Icon,
                    RightButtonContent = $"{Application.Current.Resources["DownloadUpdateDescription"]}",
                    RightButtonAction = () =>
                    {
                        App.UpdateLoader.DownloadUpdate
                        (
                            update,
                            new NotificationContent 
                            {
                                Title = $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                                Message = $"{Application.Current.Resources["LoadingUpdateDescription"]}",
                                Background = (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                                Foreground = (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"],
                                Icon = App.Icon
                            },
                            new NotificationContent
                            {
                                Title = $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                                Message = $"{Application.Current.Resources["ResultLoadingUpdateDescription1"]} {update.Version} {Application.Current.Resources["ResultLoadingUpdateDescription2"]}",
                                Background = (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                                Foreground = (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"],
                                Icon = App.Icon,
                                RightButtonContent = $"{Application.Current.Resources["InstallUpdateDescription"]}",
                                RightButtonAction = () =>
                                {
                                    Process.Start(new ProcessStartInfo 
                                    { 
                                        Arguments = "\"" + Json.GetSerializedObj(new List<InstallerInfo>
                                        {
                                            // изменить App.PathToIconAsFile на $"{App.PathToUpdateDir}\\Assets\\Icons\\DefAppIcon.ico"
                                            new InstallerInfo(App.Name, App.PathToIconAsFile, App.PathToUpdateDir,
                                            new NotificationContent
                                            {
                                                Title = $"{Application.Current.Resources["NotificationTitleUpdateDescription"]} {App.Name}",
                                                Message = $"{Application.Current.Resources["InsallationUpdateDescription"]}",
                                                Background = (SolidColorBrush)Application.Current.Resources["ContainerBackgroundBrush"],
                                                Foreground = (SolidColorBrush)Application.Current.Resources["TextForegroundBrush"]
                                            })
                                        }).Replace("\"", "\\\"") + "\"",
                                        // изменить App.PathToDir на App.PathToUpdateDir
                                        FileName = Path.Combine(App.PathToDir, $"{InstallerInfo.InstallerName}.exe") 
                                    });
                                    CloseAppComm.Execute(null);
                                },
                                CloseOnClick = true
                            }
                        );
                    },
                    CloseOnClick = true
                },
                null,
                TimeSpan.MaxValue);
            };
            App.UpdateLoader.UpdateSearchIsOver += () =>
            {
                App.UpdateLoader.StopSearchingUpdate();
            };
            App.UpdateLoader.SearchUpdate();
        }
        #endregion Method to start the application update inspector

        #endregion Private

        #region Public

        #region Method to get all shortcuts 
        public static List<Shortcut>? GetAllShortcuts(List<Section> sections)
        {
            if (sections == null)
            {
                return null;
            }

            return (from Section shortcutSection in sections
                    where shortcutSection.Shortcuts != null
                    from Shortcut shortcut in shortcutSection.Shortcuts
                    select shortcut).ToList();
        }
        #endregion Method to get all shortcuts

        #region Method to drag over shortcuts and sections
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo == null)
            {
                return;
            }

            if (dropInfo.Data == null)
            {
                return;
            }

            switch (dropInfo.Data)
            {
                case Section:
                    if (dropInfo.TargetCollection is ObservableCollection<Section> targetSections && targetSections != null)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        dropInfo.Effects = DragDropEffects.Move;
                    }
                    break;

                case Shortcut:
                    if (dropInfo.TargetItem is Section targetSection && targetSection != null && targetSection.Name != selectedSection!.Name)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Move;
                        dropInfo.EffectText = $"{Application.Current.Resources["MoveObjDescription"]}";
                        dropInfo.DestinationText = $"\"{targetSection.Name}\"";
                    }
                    break;

                default:
                    pathsToDroppingObjs.Clear();

                    foreach (string? draggedObj in ((DataObject)dropInfo.Data).GetFileDropList())
                    {
                        if (draggedObj != null)
                        {
                            pathsToDroppingObjs.Add(Path.GetFullPath(draggedObj));
                        }
                    }

                    if (dropInfo.TargetItem is Section _targetSection && _targetSection != null && pathsToDroppingObjs.Distinct().ToList().Count > 0)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<Shortcut> targetShortcuts && targetShortcuts != null &&
                        dropInfo.TargetItem is not Shortcut && pathsToDroppingObjs.Distinct().ToList().Count > 0)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }
                    break;
            }
        }
        #endregion Method to drag over shortcuts and sections

        #region Method to drop shortcuts and sections
        public void Drop(IDropInfo dropInfo)
        {
            switch (dropInfo.Data)
            {
                case Section droppingSection:
                    if (dropInfo.TargetCollection is ObservableCollection<Section> targetSections)
                    {
                        targetSections.Remove(droppingSection);
                        try
                        {
                            targetSections.Insert(dropInfo.InsertIndex, droppingSection);
                        }
                        catch
                        {
                            targetSections.Add(droppingSection);
                        }
                        SelectedSection = targetSections.ToList().Find(x => x.Name == droppingSection.Name);
                        AppDataChanged?.Invoke();
                    }
                    break;

                case Shortcut droppingShortcut:
                    if (dropInfo.TargetItem is Section targetSection)
                    {
                        Shortcut newShortcut = new(droppingShortcut) { Name = NameGenerator.Start(droppingShortcut.Name!, (from _shortcut in targetSection.Shortcuts select _shortcut.Name).ToList()) };
                        SelectedSection!.Shortcuts.Remove(droppingShortcut);
                        targetSection.Shortcuts.Add(newShortcut);
                        targetSection.Shortcuts = new ObservableCollection<Shortcut>(targetSection.Shortcuts.ToList().OrderBy(x => x.Name).ToList());
                        SelectedSection = targetSection;
                        AppDataChanged?.Invoke();
                    }
                    break;

                default:
                    if (dropInfo.TargetItem is Section _targetSection)
                    {
                        foreach (string pathToObj in pathsToDroppingObjs)
                        {
                            _targetSection.Shortcuts.Add(new Shortcut(NameGenerator.Start(Path.GetFileNameWithoutExtension(pathToObj), (from shortcut in _targetSection.Shortcuts select shortcut.Name).ToList()), pathToObj));
                        }
                        _targetSection.Shortcuts = new ObservableCollection<Shortcut>(_targetSection.Shortcuts.ToList().OrderBy(x => x.Name).ToList());
                        SelectedSection = _targetSection;                     
                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<Shortcut> targetShortcuts)
                    {
                        foreach (string pathToObj in pathsToDroppingObjs)
                        {
                            targetShortcuts.Add(new Shortcut(NameGenerator.Start(Path.GetFileNameWithoutExtension(pathToObj), (from shortcut in targetShortcuts select shortcut.Name).ToList()), pathToObj));
                        }
                        targetShortcuts = new ObservableCollection<Shortcut>(targetShortcuts.ToList().OrderBy(x => x.Name).ToList());
                        AppDataChanged?.Invoke();
                    }
                    break;
            }
        }
        #endregion Methods to drop shortcuts and sections

        #region Method for executing shortcut by click
        public static void ExecuteShortcutByClick(Shortcut shortcut)
        {
            if (shortcut == null)
            {
                return;
            }

            if (File.Exists(shortcut.PathToExetubaleObj) || Directory.Exists(shortcut.PathToExetubaleObj))
            {
                Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = "Explorer.exe" , Arguments = shortcut.PathToExetubaleObj });
            }
            else
            {
                Shortcut.ShowErrorAboutWrongPathToExetubaleObj();
            }
        }
        #endregion Method for executing shortctut by click

        #region Method for executing shortcut by hotkey
        public void ExecuteShortcutByHotKey(object? sender, HotkeyEventArgs e)
        {
            List<Shortcut>? shortcuts = GetAllShortcuts(sections.ToList());
            if (shortcuts != null && shortcuts.Count > 0)
            {
                Shortcut? existingShortcut = (from shortcut in shortcuts
                                              where shortcut.HotKey != null && shortcut.HotKey.Name == e.Name
                                              select shortcut).FirstOrDefault();
                if (existingShortcut != null)
                {
                    ExecuteShortcutByClick(existingShortcut);
                }
            }

            e.Handled = true;
        }
        #endregion Method for executing shortcut by hotkey

        #region Method for registering hotkey to shortcut
        public void RegisterHotKeyToShortcut(Shortcut shortcut, HotKey hotKey)
        {
            if (shortcut != null && hotKey != null && hotKey.Name != null && hotKey.Combination != null)
            {
                if (hotKey.Combination.MultModifierKey != Addons.MultModifierKey.None)
                {
                    List<Shortcut>? shortcuts = GetAllShortcuts(sections.ToList());
                    if (shortcuts != null && shortcuts.Count > 0)
                    {
                        if (!HotKey.ExistsWithSameCombination(hotKey.Combination, shortcuts))
                        {
                            shortcut.HotKey = hotKey;
                            HotkeyManager.Current.AddOrReplace(hotKey.Name, (Key)hotKey.Combination.Key, (ModifierKeys)hotKey.Combination.MultModifierKey, ExecuteShortcutByHotKey);
                        }
                    }
                }
                else if (shortcut.HotKey.Combination.MultModifierKey != Addons.MultModifierKey.None)
                {
                    HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                    shortcut.HotKey = hotKey;
                }
            }
        }
        #endregion Method for registering hotkey to shortcut

        #region Method for moving to shortcut placement
        public static void MoveToShortcutPlacement(Shortcut shortcut)
        {
            if (shortcut == null)
            {
                return;
            }

            if (File.Exists(shortcut.PathToExetubaleObj) || Directory.Exists(shortcut.PathToExetubaleObj))
            {
                if (File.Exists(shortcut.PathToExetubaleObj))
                {
                    appDevToolsExts.File.ShowInExplorer(shortcut.PathToExetubaleObj);
                }

                if (Directory.Exists(shortcut.PathToExetubaleObj))
                {
                    appDevToolsExts.Directory.ShowInExplorer(shortcut.PathToExetubaleObj);
                }
            }
            else
            {
                Shortcut.ShowErrorAboutWrongPathToExetubaleObj();
            }
        }
        #endregion Method for moving to shortcut placement

        #region Methods for working with renaming window
        public RenamingWindowVM CreateContextForShortcutRenamingWindow(Shortcut shortcut, List<Shortcut> shortcuts)
        {
            if (shortcut == null)
            {
                return new RenamingWindowVM();
            }

            if (shortcuts == null)
            {
                return new RenamingWindowVM();
            }

            RenamingWindowVM renamingWinContext = new()
            {
                Title = $"{Application.Current.Resources["RenamingWindowTitle"]}: {shortcut.Name}",
                Shortcut = shortcut,
                Name = shortcut.Name!
            };
            renamingWinContext.NameChanged += (name) =>
            {
                shortcut.Name = NameGenerator.Start(name, (from shortcut in shortcuts select shortcut.Name).ToList());
                AppDataChanged?.Invoke();
            };

            return renamingWinContext;
        }

        public static void ShowRenamingWindow(RenamingWindowVM renamingWinContext, string titleOwnerWin)
        {
            if (renamingWinContext == null)
            {
                return;
            }

            if (titleOwnerWin == null)
            {
                return;
            }

            new RenamingWindow
            {
                DataContext = renamingWinContext,
                Owner = appDevToolsExts.Window.Get(Application.Current, titleOwnerWin)
            }.ShowDialog();
        }
        #endregion Methods for working with renaming window

        #region Methods for working with shortcut properties window 
        public ShortcutPropertiesWindowVM CreateContextForShortcutPropertiesWindow(Shortcut shortcut)
        {
            if (shortcut == null)
            {
                return new ShortcutPropertiesWindowVM();
            }

            ShortcutPropertiesWindowVM shortcutPropertiesWinContext = new()
            {
                Title = $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}: {shortcut.Name}",
                Shortcut = new Shortcut(shortcut)
            };
            shortcutPropertiesWinContext.ShortcutChanged += (updatedShortcut) => 
            {
                shortcut.Name = updatedShortcut.Name;
                
                if (shortcut.PathToIcon != updatedShortcut.PathToIcon)
                {
                    shortcut.Icon = updatedShortcut.Icon.Clone();
                    shortcut.PathToIcon = updatedShortcut.PathToIcon;
                }

                RegisterHotKeyToShortcut(shortcut, updatedShortcut.HotKey);
                AppDataChanged?.Invoke();
            };

            return shortcutPropertiesWinContext;
        }
        public static void ShowShortcutPropertiesWindow(ShortcutPropertiesWindowVM shortcutPropertiesWinContext, string titleOwnerWin)
        {
            new ShortcutPropertiesWindow
            { 
                DataContext = shortcutPropertiesWinContext, 
                Owner = appDevToolsExts.Window.Get(Application.Current, titleOwnerWin)
            }.ShowDialog();
        }
        #endregion Methods for working with shortcut properties window

        #endregion Public

        #endregion Methods

        #region Commands

        #region Public

        #region Command for startup window
        public ICommand StartupWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {                
                    if (App.IsFirstInstance)
                    {
                        RestoreInfoAboutShortcuts();

                        if (sections != null && sections.Count > 0 && selectedSection == null)
                        {
                            SelectedSection = sections.First();
                        }

                        ShowAppComm.Execute(null);
                        AppDataChanged += () => { SaveInfoAboutShortcuts(); };

                        StartAppThemeInspector();
                        StartAppUpdateInspector();
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
                    inspectorAboutAppTheme.CancelAsync();
                }, (obj) => true);
            }
        }
        #endregion Command for closing window

        #region Commands for tray menu

        #region Command for showing application
        public ICommand ShowAppComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    Window? mainWindow = appDevToolsExts.Window.Get(Application.Current, $"{Application.Current.Resources["MainWindowTitle"]}");
                    if (mainWindow != null)
                    {
                        mainWindow.Activate();
                        mainWindow.Show();
                        mainWindow.TogglePin();
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for showing application

        #region Command for hiding application
        public ICommand HideAppComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    appDevToolsExts.Window.Hide(Application.Current, $"{Application.Current.Resources["MainWindowTitle"]}");
                }, (obj) => true);
            }
        }
        #endregion Command for hiding application

        #region Command for closing application
        public ICommand CloseAppComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    inspectorAboutAppTheme.CancelAsync();
                    Application.Current.Shutdown();
                }, (obj) => true);
            }
        }
        #endregion Command for closing application

        #region Command for showing settings window
        public ICommand ShowSettingsWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["SettingsWindowTitle"]}"))
                    {
                        new SettingsWindow { DataContext = new SettingsWindowVM() }.ShowDialog();
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for showing settings window

        #region Command for showing about program window
        public ICommand ShowAboutProgramWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["AboutProgramWindowTitle"]}"))
                    {
                        new AboutProgramWindow 
                        { 
                            DataContext = new AboutProgramWindowVM(),
                            Owner = appDevToolsExts.Window.Get(Application.Current, $"{Application.Current.Resources["MainWindowTitle"]}")
                        }.ShowDialog();
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for showing about program window

        #endregion Commands for tray menu

        #region Commands for main working panel
        
        #region Commands for working with shortcuts

        #region Command for executing shortcut
        public ICommand ExecuteShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        ExecuteShortcutByClick(shortcut);
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for exectuting shortcut

        #region Command for cutting shortcut
        public ICommand CutShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        Clipboard.SetText(Json.GetSerializedObj(new List<Shortcut> { new Shortcut(shortcut) }));
                        HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        SelectedSection!.Shortcuts.Remove(shortcut);
                        SelectedSection!.Shortcuts = new ObservableCollection<Shortcut>(selectedSection!.Shortcuts.ToList().OrderBy(x => x.Name).ToList());
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for cutting shortcut

        #region Command for copying shortcut
        public ICommand CopyShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        Clipboard.SetText(Json.GetSerializedObj(new List<Shortcut> { new Shortcut(shortcut) { HotKey = new HotKey() } }));
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for copying shortcut

        #region Command for pasting shortcut
        public ICommand PasteShortcutComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    Shortcut newShortcut = Json.GetDeserializedObj<Shortcut>(Clipboard.GetText())!.FirstOrDefault()!;
                    newShortcut.Name = NameGenerator.Start(newShortcut.Name!, (from shortcut in selectedSection!.Shortcuts select shortcut.Name).ToList());
                    newShortcut.RestoreIcon();
                    if (newShortcut.HotKey.Combination.MultModifierKey != Addons.MultModifierKey.None &&
                        HotKey.ExistsWithSameCombination(newShortcut.HotKey.Combination, selectedSection!.Shortcuts.ToList()))
                    {
                        newShortcut.HotKey = new HotKey();
                    }
                    RegisterHotKeyToShortcut(newShortcut, newShortcut.HotKey);
                    SelectedSection!.Shortcuts.Add(newShortcut);
                    SelectedSection!.Shortcuts = new ObservableCollection<Shortcut>(selectedSection!.Shortcuts.ToList().OrderBy(x => x.Name).ToList());
                    AppDataChanged?.Invoke();

                }, (obj) => 
                {
                    List<Shortcut>? shortcuts = Json.GetDeserializedObj<Shortcut>(Clipboard.GetText());
                    Shortcut? shortcut = null;
                    if (shortcuts != null)
                    {
                        shortcut = shortcuts.FirstOrDefault();
                    }

                    if (selectedSection != null && shortcut != null)
                    {
                        return true;
                    }

                    return false;
                });
            }
        }
        #endregion Command for pasting shortcut

        #region Command for renaming shortcut
        public ICommand RenameShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["RenamingWindowTitle"]}"))
                        {
                            ShowRenamingWindow(CreateContextForShortcutRenamingWindow(shortcut, selectedSection!.Shortcuts.ToList()), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for renaming shortcut

        #region Command for deleting shortcut
        public ICommand DelShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        SelectedSection!.Shortcuts.Remove(shortcut);
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for deleting shortcut

        #region Command for moving to shortcut placement
        public ICommand MoveToShortcutPlacementComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        MoveToShortcutPlacement(shortcut);
                    }
                }, (obj) => 
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for moving to shortcut placement

        #region Command for changing shortcut properties
        public ICommand ChangeShortcutPropertiesComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}"))
                        {
                            ShowShortcutPropertiesWindow(CreateContextForShortcutPropertiesWindow(shortcut), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) =>
                {
                    if (selectedShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for changing shortcut properties

        #endregion Commands for working with shortcuts

        #region Commands for working with sections

        #region Command for adding section
        public ICommand AddSectionComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    string sectionName = NameGenerator.Start($"{Application.Current.Resources["DefSectionNameDescription"]}", (from section in sections select section.Name).ToList());
                    Section newSection = new(sectionName);
                    Sections.Add(newSection);
                    SelectedSection = newSection;
                    AppDataChanged?.Invoke();
                }, (obj) => true);
            }
        }
        #endregion Command for adding section

        #region Command for duplication section
        public ICommand DuplicateSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section originalSection && originalSection != null)
                    {
                        Section newSection = new(originalSection) { Name = NameGenerator.Start(originalSection.Name!, (from section in sections select section.Name).ToList()) };
                        Sections.Add(newSection);
                        SelectedSection = newSection;
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for duplicating section

        #region Command for renaming section
        public ICommand RenameSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section section && section != null)
                    {
                        RenamingWindowVM renamingWinContext = new()
                        {
                            Title = $"{Application.Current.Resources["RenamingWindowTitle"]}: {section.Name}",
                            ShortcutSection = section,
                            Name = section.Name!
                        };
                        renamingWinContext.NameChanged += (name) => 
                        { 
                            section.Name = NameGenerator.Start(name, (from shortcutSection in sections select shortcutSection.Name).ToList());
                            AppDataChanged?.Invoke();
                        };
                        ShowRenamingWindow(renamingWinContext, $"{Application.Current.Resources["MainWindowTitle"]}");
                    }
                }, (obj) => 
                {
                    if (selectedSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for renaming section

        #region Command for deleting section
        public ICommand DelSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section section && section != null)
                    {
                        int indexSection = sections.ToList().FindIndex(x => x.Name == section.Name);
                        int countBeforeSection = GetCountBeforeSection(indexSection);

                        foreach (Shortcut shortcut in section.Shortcuts)
                        {
                            HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        }
                        Sections.Remove(section);

                        if (Sections.Count > 0)
                        {
                            if (countBeforeSection != 0)
                            {
                                SelectedSection = sections[indexSection - 1];
                            }
                            else
                            {
                                SelectedSection = sections[indexSection];
                            }
                        }

                        AppDataChanged?.Invoke();
                    }

                    int GetCountBeforeSection(int indexSection)
                    {
                        int count = 0;

                        if (indexSection != 0)
                        {
                            for (int i = 0; i < sections.Count; i++)
                            {
                                if (i != indexSection)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        return count;
                    }
                }, (obj) => 
                {
                    if (selectedSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for deleting section

        #region Command for changing color section
        public ICommand ChangeColorSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section section && section != null)
                    {
                        SolidColorBrush sectionBrush = section.Color;
                        SectionColorSelectorWindowVM sectionColorSelectorWinContext = new()
                        {
                            Title = $"{Application.Current.Resources["ChangingColorWindowTitle"]}: {section.Name}",
                            OriginalColor = sectionBrush.Color
                        };
                        sectionColorSelectorWinContext.ColorSelected += (color) => 
                        { 
                            SelectedSection!.Color = color;
                            AppDataChanged?.Invoke();
                        };
                        new SectionColorSelectorWindow
                        {
                            DataContext = sectionColorSelectorWinContext,
                            Owner = appDevToolsExts.Window.Get(Application.Current, $"{Application.Current.Resources["MainWindowTitle"]}")
                        }.ShowDialog();
                    }
                }, (obj) => 
                {
                    if (selectedSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for changing color section

        #endregion Commands for working with sections

        #endregion Commands for main working panel

        #endregion Public

        #endregion Commands

        #region Events

        #region Private
        private event Action? AppDataChanged;
        #endregion Private

        #endregion Events
    }
}
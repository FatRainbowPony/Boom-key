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
using BoomKey.Models;
using BoomKey.Views;
using GongSolutions.Wpf.DragDrop;
using NHotkey;
using NHotkey.Wpf;
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
        private ObservableCollection<FavoriteShortcut> favoriteShortcuts = new();
        private FavoriteShortcut? selectedFavoriteShortcut;
        private bool showDragDropEffectsForFavoriteShortcuts;
        private ObservableCollection<Section> shortcutSections = new();
        private Section? selectedShortcutSection;
        private NormalShortcut? selectedShortcutInSection;
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

        public ObservableCollection<FavoriteShortcut> FavoriteShortcuts
        {
            get => favoriteShortcuts;
            set => Set(ref favoriteShortcuts, value);
        }

        public FavoriteShortcut? SelectedFavoriteShortcut
        {
            get => selectedFavoriteShortcut;
            set => Set(ref selectedFavoriteShortcut, value);
        }

        public bool ShowDragDropEffectsForFavoriteShortcuts
        {
            get => showDragDropEffectsForFavoriteShortcuts;
            set => Set(ref showDragDropEffectsForFavoriteShortcuts, value);
        }

        public ObservableCollection<Section> ShortcutSections
        {
            get => shortcutSections;
            set => Set(ref shortcutSections, value);
        }

        public Section? SelectedShortcutSection
        {
            get => selectedShortcutSection;
            set => Set(ref selectedShortcutSection, value);
        }

        public NormalShortcut? SelectedShortcutInSection
        {
            get => selectedShortcutInSection;
            set => Set(ref selectedShortcutInSection, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method to restore info about shortcuts 
        private void RestoreInfoAboutShortcuts()
        {
            if (App.ShortcutsInfo == null)
            {
                return;
            }

            if (App.ShortcutsInfo.FavoriteShortcuts != null)
            {
                foreach (FavoriteShortcut favoriteShortcut in App.ShortcutsInfo.FavoriteShortcuts)
                {
                    FavoriteShortcuts.Add(favoriteShortcut);

                    if (favoriteShortcut.HotKey.Combination.MultModifierKey != Addons.MultModifierKey.None)
                    {
                        HotkeyManager.Current.AddOrReplace(favoriteShortcut.HotKey.Name, (Key)favoriteShortcut.HotKey.Combination.Key, (ModifierKeys)favoriteShortcut.HotKey.Combination.MultModifierKey, ExecuteShortcutByHotKey);
                    }
                }
            }

            if (App.ShortcutsInfo.ShortcutSections != null)
            {
                foreach (Section shorcutSection in App.ShortcutsInfo.ShortcutSections)
                {
                    ShortcutSections.Add(shorcutSection);

                    if (shorcutSection.Shortcuts != null)
                    {
                        foreach (NormalShortcut shortcut in shorcutSection.Shortcuts)
                        {
                            if (shortcut.HotKey.Combination.MultModifierKey != Addons.MultModifierKey.None)
                            {
                                HotkeyManager.Current.AddOrReplace(shortcut.HotKey.Name, (Key)shortcut.HotKey.Combination.Key, (ModifierKeys)shortcut.HotKey.Combination.MultModifierKey, ExecuteShortcutByHotKey);
                            }
                        }
                    }
                }
            }
        }
        #endregion Method to restore info about shortcut

        #region Method to save info about shortcuts
        private void SaveInfoAboutShortcuts()
        {
            appDevToolsExts.File.Save(App.PathToInfoShortcuts, new List<ShortcutsInfo> { new ShortcutsInfo(favoriteShortcuts.ToList(), shortcutSections.ToList()) });
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

        #endregion Private

        #region Public

        #region Method to get a general list of shortcuts 
        public static List<Shortcut>? GetGeneralListOfShortcuts(List<FavoriteShortcut> favoriteShortcuts, List<Section> shortcutSections)
        {
            List<Shortcut>? shortcuts = null;

            if (favoriteShortcuts != null || shortcutSections != null)
            {
                shortcuts = new List<Shortcut>();

                if (favoriteShortcuts != null)
                {
                    shortcuts.AddRange(from favoriteShortcut in favoriteShortcuts select favoriteShortcut);
                }

                if (shortcutSections != null)
                {
                    shortcuts.AddRange(from Section shortcutSection in shortcutSections
                                       where shortcutSection.Shortcuts != null
                                       from NormalShortcut shortcut in shortcutSection.Shortcuts
                                       select shortcut);
                }
            }

            return shortcuts;
        }
        #endregion Method to get a general list of shortcuts

        #region Method to drag over shortcuts and shortcut sections
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
                case FavoriteShortcut:
                    if (dropInfo.TargetItem is Section targetSectionForFavoriteShortcut && targetSectionForFavoriteShortcut != null)
                    {
                        ShowDragDropEffectsForFavoriteShortcuts = true;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                        dropInfo.EffectText = $"{Application.Current.Resources["AddObjDescription"]}";
                        dropInfo.DestinationText = $"\"{targetSectionForFavoriteShortcut.Name}\"";
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForFavoriteShortcut && targetFavoriteShortcutsForFavoriteShortcut != null)
                    {
                        ShowDragDropEffectsForFavoriteShortcuts = false;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        dropInfo.Effects = DragDropEffects.Move;
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<NormalShortcut> targetShortcutsForFavoriteShortcut && targetShortcutsForFavoriteShortcut != null &&
                        dropInfo.TargetItem is not NormalShortcut)
                    {
                        ShowDragDropEffectsForFavoriteShortcuts = true;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                        dropInfo.EffectText = $"{Application.Current.Resources["AddObjDescription"]}";
                        dropInfo.DestinationText = $"\"{selectedShortcutSection!.Name}\"";
                    }
                    break;

                case NormalShortcut:
                    if (dropInfo.TargetItem is Section targetSectionForShortcutForShortcut && targetSectionForShortcutForShortcut != null && targetSectionForShortcutForShortcut.Name != selectedShortcutSection!.Name)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                        dropInfo.EffectText = $"{Application.Current.Resources["AddObjDescription"]}";
                        dropInfo.DestinationText = $"\"{targetSectionForShortcutForShortcut.Name}\"";
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForShortcut && targetFavoriteShortcutsForShortcut != null &&
                        dropInfo.TargetItem is not FavoriteShortcut)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                        dropInfo.EffectText = $"{Application.Current.Resources["AddObjDescription"]}";
                        dropInfo.DestinationText = $"{((string)Application.Current.Resources["FavoriteShortcutsDescription"]).ToLower()}";
                    }
                    break;

                case Section:
                    if (dropInfo.TargetCollection is ObservableCollection<Section> targetShortcutSectionsForShortcutSection && targetShortcutSectionsForShortcutSection != null)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        dropInfo.Effects = DragDropEffects.Move;
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

                    if (dropInfo.TargetItem is Section targetShortcutSectionForDef && targetShortcutSectionForDef != null &&
                        pathsToDroppingObjs.Distinct().ToList().Count > 0)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForDef && targetFavoriteShortcutsForDef != null &&
                        dropInfo.TargetItem is not FavoriteShortcut && pathsToDroppingObjs.Distinct().ToList().Count > 0)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<NormalShortcut> targetShortcutsForDef && targetShortcutsForDef != null &&
                        dropInfo.TargetItem is not NormalShortcut && pathsToDroppingObjs.Distinct().ToList().Count > 0)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                        dropInfo.Effects = DragDropEffects.Copy;
                    }
                    break;
            }
        }
        #endregion Method to drag over shortcuts and shortcut sections

        #region Method to drop shortcuts and shortcut sections
        public void Drop(IDropInfo dropInfo)
        {
            switch (dropInfo.Data)
            {
                case FavoriteShortcut favoriteShortcut:
                    if (dropInfo.TargetItem is Section targetSectionForFavoriteShortcut)
                    {
                        targetSectionForFavoriteShortcut.Shortcuts.Add(new(favoriteShortcut) { Name = NameGenerator.Start(favoriteShortcut.Name!, (from shortcut in targetSectionForFavoriteShortcut.Shortcuts select shortcut.Name).ToList()) });
                        targetSectionForFavoriteShortcut.Shortcuts.ToList().OrderBy(x => x.Name);
                        SelectedShortcutSection = targetSectionForFavoriteShortcut;
                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForFavoriteShortcut)
                    {
                        targetFavoriteShortcutsForFavoriteShortcut.Remove(favoriteShortcut);
                        try
                        {
                            targetFavoriteShortcutsForFavoriteShortcut.Insert(dropInfo.InsertIndex, favoriteShortcut);
                        }
                        catch
                        {
                            targetFavoriteShortcutsForFavoriteShortcut.Add(favoriteShortcut);
                        }

                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<NormalShortcut> targetShortcutsForFavoriteShortcut)
                    {          
                        targetShortcutsForFavoriteShortcut.Add(new(favoriteShortcut) { Name = NameGenerator.Start(favoriteShortcut.Name!, (from shortcut in targetShortcutsForFavoriteShortcut select shortcut.Name).ToList()) });
                        targetShortcutsForFavoriteShortcut.ToList().OrderBy(x => x.Name);
                        AppDataChanged?.Invoke();
                    }
                    break;

                case NormalShortcut shortcutInSection:
                    if (dropInfo.TargetItem is Section targetSectionForShortcut)
                    {
                        targetSectionForShortcut.Shortcuts.Add(new(shortcutInSection) { Name = NameGenerator.Start(shortcutInSection.Name!, (from shortcut in targetSectionForShortcut.Shortcuts select shortcut.Name).ToList()) });
                        targetSectionForShortcut.Shortcuts.ToList().OrderBy(x => x.Name);
                        SelectedShortcutSection = targetSectionForShortcut;
                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForShortcut)
                    {
                        targetFavoriteShortcutsForShortcut.Add(new(shortcutInSection) { Name = NameGenerator.Start(shortcutInSection.Name!, (from favoriteShortcut in targetFavoriteShortcutsForShortcut select favoriteShortcut.Name).ToList()) });
                        AppDataChanged?.Invoke();
                    }
                    break;

                case Section shortcutSection:
                    if (dropInfo.TargetCollection is ObservableCollection<Section> targetShortcutSectionsForShortcutSection)
                    {
                        targetShortcutSectionsForShortcutSection.Remove(shortcutSection);
                        try
                        {
                            targetShortcutSectionsForShortcutSection.Insert(dropInfo.InsertIndex, shortcutSection);
                        }
                        catch
                        {
                            targetShortcutSectionsForShortcutSection.Add(shortcutSection);
                        }
                        SelectedShortcutSection = targetShortcutSectionsForShortcutSection.ToList().Find(x => x.Name == shortcutSection.Name);
                        AppDataChanged?.Invoke();
                    }
                    break;

                default:
                    if (dropInfo.TargetItem is Section targetShortcutSectionForDef)
                    {
                        foreach (string pathToObj in pathsToDroppingObjs)
                        {
                            targetShortcutSectionForDef.Shortcuts.Add(new NormalShortcut(NameGenerator.Start(Path.GetFileNameWithoutExtension(pathToObj), (from shortcut in targetShortcutSectionForDef.Shortcuts select shortcut.Name).ToList()), pathToObj));
                        }
                        targetShortcutSectionForDef.Shortcuts.ToList().OrderBy(x => x.Name);
                        SelectedShortcutSection = targetShortcutSectionForDef;                     
                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<FavoriteShortcut> targetFavoriteShortcutsForDef)
                    {
                        foreach (string pathToObj in pathsToDroppingObjs)
                        {
                            targetFavoriteShortcutsForDef.Add(new FavoriteShortcut(NameGenerator.Start(Path.GetFileNameWithoutExtension(pathToObj), (from favoriteShortcut in targetFavoriteShortcutsForDef select favoriteShortcut.Name).ToList()), pathToObj));
                        }
                        
                        AppDataChanged?.Invoke();
                    }

                    if (dropInfo.TargetCollection is ObservableCollection<NormalShortcut> targetShortcutsForDef)
                    {
                        foreach (string pathToObj in pathsToDroppingObjs)
                        {
                            targetShortcutsForDef.Add(new NormalShortcut(NameGenerator.Start(Path.GetFileNameWithoutExtension(pathToObj), (from shortcut in targetShortcutsForDef select shortcut.Name).ToList()), pathToObj));
                        }
                        targetShortcutsForDef.ToList().OrderBy(x => x.Name);             
                        AppDataChanged?.Invoke();
                    }
                    break;
            }
        }
        #endregion Methods to drop shortcuts and shortcut sections

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
            List<Shortcut>? shortcuts = GetGeneralListOfShortcuts(favoriteShortcuts.ToList(), shortcutSections.ToList());
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
                    List<Shortcut>? shortcuts = GetGeneralListOfShortcuts(favoriteShortcuts.ToList(), shortcutSections.ToList());
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
        public RenamingWindowVM CreateContextForShortcutRenamingWindow(Shortcut shortcut, object shortcuts)
        {
            ObservableCollection<FavoriteShortcut> favoriteShortcuts;
            ObservableCollection<NormalShortcut> shortcutsInSection;

            if (shortcut == null)
            {
                return new RenamingWindowVM();
            }

            if (shortcuts is not ObservableCollection<FavoriteShortcut> && shortcuts is not ObservableCollection<NormalShortcut>)
            {
                return new RenamingWindowVM();
            }

            RenamingWindowVM renamingWinContext = new()
            {
                Title = $"{Application.Current.Resources["RenamingWindowTitle"]}: {shortcut.Name}",
                Shortcut = shortcut,
                Name = shortcut.Name!
            };

            switch (shortcuts)
            {
                case ObservableCollection<FavoriteShortcut>:
                    favoriteShortcuts = (ObservableCollection<FavoriteShortcut>)shortcuts;
                    renamingWinContext.NameChanged += (name) => 
                    { 
                        shortcut.Name = NameGenerator.Start(name, (from shortcut in favoriteShortcuts select shortcut.Name).ToList());
                        AppDataChanged?.Invoke();
                    };
                    break;

                case ObservableCollection<NormalShortcut>:
                    shortcutsInSection = (ObservableCollection<NormalShortcut>)shortcuts;
                    renamingWinContext.NameChanged += (name) => 
                    { 
                        shortcut.Name = NameGenerator.Start(name, (from shortcut in shortcutsInSection select shortcut.Name).ToList());
                        AppDataChanged?.Invoke();
                    };
                    break;
            }

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
                Shortcut = new FavoriteShortcut(shortcut)
            };
            shortcutPropertiesWinContext.ShortcutChanged += (updatedShortcut) => 
            {
                shortcut.Name = updatedShortcut.Name;
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

                        if (shortcutSections != null && shortcutSections.Count > 0 && selectedShortcutSection == null)
                        {
                            SelectedShortcutSection = shortcutSections.First();
                        }

                        ShowAppComm.Execute(null);
                        AppDataChanged += () => { SaveInfoAboutShortcuts(); };
                        StartAppThemeInspector();
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

        #region Command for executing shortcut by click
        public ICommand ExecuteShortcutByClickComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Shortcut shortcut && shortcut != null)
                    {
                        ExecuteShortcutByClick(shortcut);
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for exectuting shortcut by click

        #region Command for executing favorite shortcut by selection in menu
        public ICommand ExecuteFavoriteShortcutBySelectionInMenuComm
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
                    if (selectedFavoriteShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for executing favorite shortcut by selection in menu

        #region Command for executing shortcut in section by selection in menu
        public ICommand ExecuteShortcutInSectionBySelectionInMenuComm
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
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for executing shortcut in section by selection in menu

        #region Command for renaming favorite shortcut
        public ICommand RenameFavoriteShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut favoriteShortcut && favoriteShortcut != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["RenamingWindowTitle"]}"))
                        {
                            ShowRenamingWindow(CreateContextForShortcutRenamingWindow(favoriteShortcut, favoriteShortcuts), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) => 
                {
                    if (selectedFavoriteShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for renaming favorite shortcut

        #region Command for renaming shortcut in section
        public ICommand RenameShortcutInSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is NormalShortcut shortcutInSection && shortcutInSection != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["RenamingWindowTitle"]}"))
                        {
                            ShowRenamingWindow(CreateContextForShortcutRenamingWindow(shortcutInSection, selectedShortcutSection!.Shortcuts), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) => 
                {
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for renaming shortcut in section

        #region Command for adding shortcut in favorite
        public ICommand AddShortcutInFavoriteComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is NormalShortcut shortcut && shortcut != null)
                    {
                        FavoriteShortcuts.Add(new FavoriteShortcut(shortcut) { Name = NameGenerator.Start(shortcut.Name!, (from shortcutInSection in selectedShortcutSection!.Shortcuts select shortcutInSection.Name).ToList()) });
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for adding shortcut in favorite

        #region Command for deleting shortcut from favorite
        public ICommand DelShortcutFromFavoriteComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut shortcut && shortcut != null)
                    {
                        HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        FavoriteShortcuts.Remove(shortcut);
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedFavoriteShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for deleting shortcut from favorite

        #region Command for deleting shortcut from section
        public ICommand DelShortcutFromSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is NormalShortcut shortcut && shortcut != null)
                    {
                        HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        SelectedShortcutSection!.Shortcuts.Remove(shortcut);
                        AppDataChanged?.Invoke();
                    }
                }, (obj) => 
                {
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for deleting shortcut from section

        #region Command for moving to favorite shortcut placement
        public ICommand MoveToFavoriteShortcutPlacementComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut favoriteShortcut && favoriteShortcut != null)
                    {
                        MoveToShortcutPlacement(favoriteShortcut);
                    }
                }, (obj) => 
                {
                    if (selectedFavoriteShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for moving to favorite shortcut placement

        #region Command for moving to shortcut in section placement
        public ICommand MoveToShortcutInSectionPlacementComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is NormalShortcut shortcut && shortcut != null)
                    {
                        MoveToShortcutPlacement(shortcut);
                    }
                }, (obj) => 
                {
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for moving to shortcut in section placement

        #region Command for changing favorite shortcut properties
        public ICommand ChangeFavoriteShortcutPropertiesComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut favoriteShortcut && favoriteShortcut != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}"))
                        {
                            ShowShortcutPropertiesWindow(CreateContextForShortcutPropertiesWindow(favoriteShortcut), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) =>
                {
                    if (selectedFavoriteShortcut == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for changing favorite shortcut properties

        #region Command for changing shortcut in section properties
        public ICommand ChangeShortcutInSectionPropertieComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is NormalShortcut shortcutInSection && shortcutInSection != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}"))
                        {
                            ShowShortcutPropertiesWindow(CreateContextForShortcutPropertiesWindow(shortcutInSection), $"{Application.Current.Resources["MainWindowTitle"]}");
                        }
                    }
                }, (obj) =>
                {
                    if (selectedShortcutInSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for changing shortcut in section properties window

        #region Command for opening favorite shortcuts in window
        public ICommand OpenFavoriteShortcutsInWinComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["FavoriteShortcutsTitle"]}"))
                    {
                        FavoriteShortcutsWindowVM favoriteShortcutsWinContext = new() { MainWinContext = this };
                        FavoriteShortcutsWindow favoriteShortcutsWin = new() { DataContext = favoriteShortcutsWinContext };
                        favoriteShortcutsWin.Show();
                        favoriteShortcutsWin.TogglePin();
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for showing favorite shortcuts in window

        #endregion Commands for working with shortcuts

        #region Commands for working with shortcut section

        #region Command for renaming shortcut section
        public ICommand RenameShortcutSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section shortcutSection && shortcutSection != null)
                    {
                        RenamingWindowVM renamingWinContext = new()
                        {
                            Title = $"{Application.Current.Resources["RenamingWindowTitle"]}: {shortcutSection.Name}",
                            ShortcutSection = shortcutSection,
                            Name = shortcutSection.Name!
                        };
                        renamingWinContext.NameChanged += (name) => 
                        { 
                            shortcutSection.Name = NameGenerator.Start(name, (from shortcutSection in shortcutSections select shortcutSection.Name).ToList());
                            AppDataChanged?.Invoke();
                        };
                        ShowRenamingWindow(renamingWinContext, $"{Application.Current.Resources["MainWindowTitle"]}");
                    }
                }, (obj) => 
                {
                    if (selectedShortcutSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for renaming shortcut section

        #region Command for adding shortcut section
        public ICommand AddShortcutSectionComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    Section newSection = new(NameGenerator.Start($"{Application.Current.Resources["DefShortcutSectionNameDescription"]}", (from shortcutSection in shortcutSections select shortcutSection.Name).ToList()));
                    ShortcutSections.Add(newSection);
                    SelectedShortcutSection = newSection;
                    AppDataChanged?.Invoke();
                }, (obj) => true);
            }
        }
        #endregion Command for adding shortcut section

        #region Command for deleting shortcut sections
        public ICommand DelShortcutSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section shortcutSection && shortcutSection != null)
                    {
                        int indexShortcutSection = shortcutSections.ToList().FindIndex(x => x.Name == shortcutSection.Name);
                        int countBeforeShortcutSection = GetCountBeforeShortcutSection(indexShortcutSection);

                        foreach (NormalShortcut shortcut in shortcutSection.Shortcuts)
                        {
                            HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        }
                        ShortcutSections.Remove(shortcutSection);

                        if (ShortcutSections.Count > 0)
                        {
                            if (countBeforeShortcutSection != 0)
                            {
                                SelectedShortcutSection = shortcutSections[indexShortcutSection - 1];
                            }
                            else
                            {
                                SelectedShortcutSection = shortcutSections[indexShortcutSection];
                            }
                        }

                        AppDataChanged?.Invoke();
                    }

                    int GetCountBeforeShortcutSection(int indexShortcutSection)
                    {
                        int count = 0;

                        if (indexShortcutSection != 0)
                        {
                            for (int i = 0; i < shortcutSections.Count; i++)
                            {
                                if (i != indexShortcutSection)
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
                    if (selectedShortcutSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for deleting shortcut sections

        #region Command for changing color shortcut section
        public ICommand ChangeColorShortcutSectionComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is Section shortcutSection && shortcutSection != null)
                    {
                        SolidColorBrush shortcutSectionBrush = shortcutSection.Color;
                        SectionColorSelectorWindowVM sectionColorSelectorWinContext = new()
                        {
                            Title = $"{Application.Current.Resources["ChangingColorWindowTitle"]}: {shortcutSection.Name}",
                            OriginalColor = shortcutSectionBrush.Color
                        };
                        sectionColorSelectorWinContext.ColorSelected += (color) => 
                        { 
                            selectedShortcutSection!.Color = color;
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
                    if (selectedShortcutSection == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command for changing color shortcut section

        #endregion Commands for working with shortcut section

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
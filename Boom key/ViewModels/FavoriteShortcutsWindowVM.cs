using System;
using System.Windows;
using System.Windows.Input;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using BoomKey.Models;
using GongSolutions.Wpf.DragDrop;
using NHotkey.Wpf;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class FavoriteShortcutsWindowVM : ViewModel, IDropTarget
    {
        #region Fields

        #region Private
        private Application currentApplication = Application.Current;
        private string pathToWindowInfo = App.PathToInfoSubWindow;
        private MainWindowVM? mainWinContext;
        private FavoriteShortcut? selectedShortcut;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public Application CurrentApplication
        {
            get => currentApplication;
            set => Set(ref currentApplication, value);
        }

        public string PathToWindowInfo
        {
            get => pathToWindowInfo;
            set => Set(ref pathToWindowInfo, value);
        }

        public MainWindowVM? MainWinContext
        {
            get => mainWinContext;
            set => Set(ref mainWinContext, value);
        }

        public FavoriteShortcut? SelectedShortcut
        {
            get => selectedShortcut;
            set => Set(ref selectedShortcut, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method to drag over shortcuts
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (mainWinContext != null)
            {
                mainWinContext.DragOver(dropInfo);
            }
        }
        #endregion Method to drag over shortcuts

        #region Method to drop shortcuts
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (mainWinContext != null)
            {
                mainWinContext.Drop(dropInfo);
            }
        }
        #endregion Method to drop shortcuts

        #endregion Private

        #endregion Methods

        #region Commands

        #region Public

        #region Command for executing shortcut by click
        public ICommand ExecuteShortcutByClickComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut shortcut && shortcut != null)
                    {
                        MainWindowVM.ExecuteShortcutByClick(shortcut);
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for executing shortcut by clikc

        #region Command for executing shortcut by selection in menu
        public ICommand ExecuteShortcutBySelectionInMenuComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut shortcut && shortcut != null)
                    {
                        MainWindowVM.ExecuteShortcutByClick(shortcut);
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
        #endregion Command for executing shortcut by selection in menu

        #region Command for renaming shortcut
        public ICommand RenameShortcutComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter is FavoriteShortcut shortcut && shortcut != null && MainWinContext != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["RenamingWindowTitle"]}"))
                        {
                            MainWindowVM.ShowRenamingWindow(MainWinContext.CreateContextForShortcutRenamingWindow(shortcut, mainWinContext!.FavoriteShortcuts), $"{Application.Current.Resources["FavoriteShortcutsTitle"]}");
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
                    if (parameter is FavoriteShortcut shortcut && shortcut != null && MainWinContext != null)
                    {
                        HotkeyManager.Current.Remove(shortcut.HotKey.Name);
                        MainWinContext.FavoriteShortcuts.Remove(shortcut);
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
                    if (parameter is FavoriteShortcut shortcut && shortcut != null)
                    {
                        MainWindowVM.MoveToShortcutPlacement(shortcut);
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
                    if (parameter is FavoriteShortcut shortcut && shortcut != null && mainWinContext != null)
                    {
                        if (!appDevToolsExts.Window.IsOpened(Application.Current, $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}"))
                        {
                            MainWindowVM.ShowShortcutPropertiesWindow(mainWinContext.CreateContextForShortcutPropertiesWindow(shortcut), $"{Application.Current.Resources["FavoriteShortcutsTitle"]}");
                        }
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for changing shortcut properties

        #endregion Public

        #endregion Commands
    }
}
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using BoomKey.Models;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class ShortcutPropertiesWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private string? title;
        private Shortcut? shortcut;
        private ObservableCollection<string> keys = new(appDevToolsExts.Enum.GetMemberValues<Addons.Key>());
        private string? selectedKey;
        private ObservableCollection<string> multModifierKeys = new(appDevToolsExts.Enum.GetMemberValues<Addons.MultModifierKey>());
        private string? selectedMultModifierKey;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string? Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public Shortcut? Shortcut
        {
            get => shortcut;
            set => Set(ref shortcut, value);
        }

        public ObservableCollection<string> Keys
        {
            get => keys;
            set => Set(ref keys, value);
        }

        public string? SelectedKey
        {
            get => selectedKey;
            set => Set(ref selectedKey, value);
        }

        public ObservableCollection<string> MultModifierKeys
        {
            get => multModifierKeys;
            set => Set(ref multModifierKeys, value);
        }

        public string? SelectedMultModifierKey
        {
            get => selectedMultModifierKey;
            set => Set(ref selectedMultModifierKey, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private

        #region Method to close shortcut properties window
        private void CloseShortcutPropertiesWindow()
        {
            appDevToolsExts.Window.Close(Application.Current, title!);
        }
        #endregion Method to close shortcut properties window

        #endregion Private

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
                    Title ??= $"{Application.Current.Resources["ShortcutPropertiesWindowTitle"]}";

                    if (shortcut != null)
                    {
                        SelectedKey = appDevToolsExts.Enum.GetMemberValue(shortcut.HotKey.Combination.Key);
                        SelectedMultModifierKey = appDevToolsExts.Enum.GetMemberValue(shortcut.HotKey.Combination.MultModifierKey);
                    }
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #region Command to select shortcut icon
        public ICommand SelectShortcutIconComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    string? pathToShortcutIcon = appDevToolsExts.File.UseOpenDialogSingleSel($"{Application.Current.Resources["SelectShortcutIconDescription"]}", $"{Application.Current.Resources["FilterForShortcutIconDescription"]}");
                    if (pathToShortcutIcon != null && File.Exists(pathToShortcutIcon))
                    {
                        ImageSource? shortcutIcon = appDevToolsExts.File.GetIcon(pathToShortcutIcon);
                        if (shortcutIcon != null)
                        {
                            Shortcut!.Icon = shortcutIcon;
                            Shortcut!.PathToIcon = pathToShortcutIcon;
                        }
                        else
                        {
                            Shortcut.ShowErrorAboutFailedGettingIcon();
                        }
                    }
                }, (obj) => true);
            }
        }
        #endregion Command to select shortcut icon

        #region Command to unconfirm changes shortcut properties
        public ICommand UnconfirmChangesShortcutPropertiesComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    CloseShortcutPropertiesWindow();
                }, (obj) => true);
            }
        }
        #endregion Command to unconfirm changes shortcut properties 

        #region Command to confirm changes shortcut properties
        public ICommand ConfirmChangesShortcutPropertiesComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    if (!string.IsNullOrEmpty(shortcut!.Name) && !string.IsNullOrWhiteSpace(shortcut!.Name))
                    {
                        Shortcut!.Name = shortcut.Name.Trim();
                    }

                    Shortcut!.HotKey = new HotKey(shortcut.HotKey.Name, new Combination(appDevToolsExts.Enum.GetEnumFromStr<Addons.Key>(selectedKey!), appDevToolsExts.Enum.GetEnumFromStr<Addons.MultModifierKey>(selectedMultModifierKey!)));
                    ShortcutChanged?.Invoke(shortcut);

                    CloseShortcutPropertiesWindow();
                }, (obj) => true);
            }
        }
        #endregion Command to confirm changes shortcut properties

        #endregion Public

        #endregion Commands

        #region Events

        #region Public
        public event Action<Shortcut>? ShortcutChanged;
        #endregion Public

        #endregion Events
    }
}
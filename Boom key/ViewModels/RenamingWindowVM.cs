using System;
using System.Windows;
using System.Windows.Input;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using BoomKey.Models;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class RenamingWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private string? title;
        private Shortcut? shortcut;
        private Section? shortcutSection;
        private string? name;
        private string? oldName;
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

        public Section? ShortcutSection
        {
            get => shortcutSection;
            set => Set(ref shortcutSection, value);
        }

        public string? Name
        {
            get => name;
            set => Set(ref name, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void CloseRenamingWindow()
        {
            appDevToolsExts.Window.Close(Application.Current, title!);
        }
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
                    Title ??= $"{Application.Current.Resources["RenamingWindowTitle"]}";

                    if (shortcut != null)
                    {
                        oldName = shortcut.Name;
                    }

                    if (shortcutSection != null)
                    {
                        oldName = shortcutSection.Name;
                    }

                    Name = oldName;
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #region Command to unconfirm changes name
        public ICommand UnconfirmChangesNameComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    CloseRenamingWindow();
                }, (obj) => true);
            }
        }
        #endregion Command to unconfirm changes name

        #region Command to confirm changes name
        public ICommand ConfirmChangesNameComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    NameChanged?.Invoke(name!);
                    CloseRenamingWindow();
                }, (obj) =>
                {
                    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || name.Trim() == oldName)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command to confirm changes name

        #endregion Public

        #endregion Commands

        #region Events

        #region Public
        public event Action<string>? NameChanged;
        #endregion Public

        #endregion Events
    }
}
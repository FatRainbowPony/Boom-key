using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class ColorPickerWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private Color currentColor;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public Color CurrentColor
        {
            get => currentColor;
            set => Set(ref currentColor, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private static void CloseColorPickerWindow()
        {
            appDevToolsExts.Window.Close(Application.Current, $"{Application.Current.Resources["ColorPickerWindowTitle"]}");
        }
        #endregion Private

        #endregion Methods

        #region Commands 

        #region Public

        #region Commad for getting selected color
        public ICommand GetSelectedColorComm
        {
            get
            {
                return new CommandsVM((parameter) =>
                {
                    if (parameter != null && parameter is Color color)
                    {
                        CurrentColor = color;
                    }
                }, (obj) => true);
            }
        }
        #endregion Commad for getting selected color

        #region Command to unconfirm selected color
        public ICommand UnconfirmSelectedColorComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    CloseColorPickerWindow();
                }, (obj) => true);
            }
        }
        #endregion Command to unconfirm selected color

        #region Command to confirm selected color
        public ICommand ConfirmSelectedColorComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    ColorChanged?.Invoke(currentColor);
                    CloseColorPickerWindow();
                }, (obj) => true);
            }
        }
        #endregion Command to confirm selected color

        #endregion Public

        #endregion Commands 

        #region Events

        #region Public
        public event Action<Color>? ColorChanged;
        #endregion PUblic

        #endregion Events
    }
}
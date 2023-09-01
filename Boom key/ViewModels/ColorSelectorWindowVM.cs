using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Templates.MVVM.ViewModel.Commands;
using BoomKey.Views;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.ViewModels
{
    public class ColorSelectorWindowVM : ViewModel
    {
        #region Fields

        #region Private      
        private string? title;
        private ObservableCollection<SolidColorBrush>? colors;
        private SolidColorBrush? selectedColor;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string? Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public Color OriginalColor { get; set; }

        public ObservableCollection<SolidColorBrush>? Colors
        {
            get => colors;
            set => Set(ref colors, value);
        }

        public SolidColorBrush? SelectedColor
        {
            get => selectedColor;
            set => Set(ref selectedColor, value);
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void CloseSectionColorsSelectorWindow()
        {
            appDevToolsExts.Window.Close(Application.Current, title!);
        }

        private bool SameColorExist(Color color)
        {
            int countExistingColors = (from brush in colors where  brush.Color.R == color.R &&  brush.Color.G == color.G &&  brush.Color.B == color.B &&  brush.Color.A == color.A select brush).Count();
            if (countExistingColors > 0)
            {
                return true;
            }

            return false;
        }

        private SolidColorBrush? FindColor(Color color)
        {
            return colors!.ToList().Find(x => x.Color.R == color.R && x.Color.G == color.G && x.Color.B == color.B && x.Color.A == color.A);
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
                    Title ??= $"{Application.Current.Resources["ChangingColorWindowTitle"]}";

                    List<SolidColorBrush>? brushes = appDevToolsExts.File.Load<SolidColorBrush>(App.PathToInfoColors);
                    if (brushes != null && brushes.FirstOrDefault() != null)
                    {
                        Colors = new ObservableCollection<SolidColorBrush>(brushes);
                    }
                    else
                    {
                        Colors = new ObservableCollection<SolidColorBrush>(App.PersonalizationColors);
                    }
                    SelectedColor = FindColor(OriginalColor);
                }, (obj) => true);
            }
        }
        #endregion Command for startup window

        #region Command for adding color
        public ICommand AddColorComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    Color oldColor;
                    
                    if (selectedColor != null)
                    {
                        oldColor = Color.FromRgb(selectedColor.Color.R, selectedColor.Color.G, selectedColor.Color.B);
                    }

                    ColorPickerWindowVM colorPickerWinContext = new() { CurrentColor = oldColor };
                    colorPickerWinContext.ColorChanged += (color) => 
                    {
                        SolidColorBrush newColor = new(color);
                        if (!SameColorExist(newColor.Color))
                        {
                            Colors!.Add(newColor);
                            appDevToolsExts.File.Save(App.PathToInfoColors, colors);
                            SelectedColor = newColor;
                        }
                        else
                        {
                            SolidColorBrush? existingColor = FindColor(color);
                            if (existingColor != null)
                            {
                                SelectedColor = existingColor;
                            }
                            else
                            {
                                SelectedColor = newColor;
                            }
                        }
                    };
                    new ColorPickerWindow() 
                    { 
                        DataContext = colorPickerWinContext,
                        Owner = appDevToolsExts.Window.Get(Application.Current, title!)
                    }.ShowDialog();
                }, (obj) => true);
            }
        }
        #endregion Command for adding color

        #region Command to unconfirm selected color
        public ICommand UnconfirmSelectedColorComm
        {
            get
            {
                return new CommandsVM((obj) =>
                {
                    CloseSectionColorsSelectorWindow();
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
                    ColorSelected?.Invoke(new SolidColorBrush(selectedColor!.Color));
                    CloseSectionColorsSelectorWindow();
                }, (obj) => 
                {
                    if (selectedColor == null)
                    {
                        return false;
                    }

                    return true;
                });
            }
        }
        #endregion Command to confirm selected color

        #endregion Public

        #endregion Commands

        #region Events

        #region Public
        public event Action<SolidColorBrush>? ColorSelected;
        #endregion Public

        #endregion Events
    }
}
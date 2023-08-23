using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using appDevToolsExts = AppDevTools.Extensions;

namespace BoomKey.Models
{
    public class Section : ViewModel
    {
        #region Fields

        #region Private
        private string? name;
        private SolidColorBrush color;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string? Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public SolidColorBrush Color
        {
            get => color;
            set => Set(ref color, value);
        }

        public ObservableCollection<NormalShortcut> Shortcuts { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Section()
        {
            Color = GenerateColor();
            Shortcuts = new ObservableCollection<NormalShortcut>();
        }

        public Section(string name)
        {
            Name = name;
            Color = GenerateColor();
            Shortcuts = new ObservableCollection<NormalShortcut>();
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private static SolidColorBrush GenerateColor()
        {
            List<Color> colors;
            List<SolidColorBrush>? brushes = null;
            try
            {
                brushes = appDevToolsExts.File.Load<SolidColorBrush>(App.PathToInfoColors);
            }
            catch { }
            if (brushes != null && brushes.FirstOrDefault() != null)
            {
                colors = (from brush in brushes select System.Windows.Media.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B)).ToList();
            }
            else
            {
                colors = Addons.Colors.GetAll();
            }

            for (int i = 0; i < colors.Count; i++)
            {
                if ((colors[i].R == 0 && colors[i].G == 0 && colors[i].B == 0 && colors[i].A == 255) ||
                    (colors[i].R == 255 && colors[i].G == 255 && colors[i].B == 255 && colors[i].A == 255))
                {
                    colors.Remove(colors[i]);
                }
            }

            return new SolidColorBrush(colors[new Random().Next(colors.Count)]);
        }
        #endregion Private

        #endregion Methods
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppDevTools.Controls.CheckBoxs
{
    public class SwitcherCheckBox : CheckBox
    {
        #region Properties

        #region Public

        #region Property for track border thickness
        public double TrackBorderThinckness
        {
            get { return (double)GetValue(TrackBorderThincknessProperty); }
            set { SetValue(TrackBorderThincknessProperty, value); }
        }

        public static readonly DependencyProperty TrackBorderThincknessProperty =
            DependencyProperty.Register(nameof(TrackBorderThinckness), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for track border thickness

        #region Property for track width
        public double TrackWidth
        {
            get { return (double)GetValue(TrackWidthProperty); }
            set { SetValue(TrackWidthProperty, value); }
        }

        public static readonly DependencyProperty TrackWidthProperty =
            DependencyProperty.Register(nameof(TrackWidth), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for track width

        #region Property for track height
        public double TrackHeight
        {
            get { return (double)GetValue(TrackHeightProperty); }
            set { SetValue(TrackHeightProperty, value); }
        }

        public static readonly DependencyProperty TrackHeightProperty =
            DependencyProperty.Register(nameof(TrackHeight), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for track height

        #region Property for track rounding corners
        public double TrackRoundingCorners
        {
            get { return (double)GetValue(TrackRoundingCornersProperty); }
            set { SetValue(TrackRoundingCornersProperty, value); }
        }

        public static readonly DependencyProperty TrackRoundingCornersProperty =
            DependencyProperty.Register(nameof(TrackRoundingCorners), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for track rounding corners

        #region Property for switch background
        public SolidColorBrush SwitchBackground
        {
            get { return (SolidColorBrush)GetValue(SwitchBackgroundProperty); }
            set { SetValue(SwitchBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SwitchBackgroundProperty =
            DependencyProperty.Register(nameof(SwitchBackground), typeof(SolidColorBrush), typeof(SwitcherCheckBox), new PropertyMetadata(null));
        #endregion Property for switch background

        #region Property for switch border brush
        public SolidColorBrush SwitchBorderBrush
        {
            get { return (SolidColorBrush)GetValue(SwitchBorderBrushProperty); }
            set { SetValue(SwitchBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty SwitchBorderBrushProperty =
            DependencyProperty.Register(nameof(SwitchBorderBrush), typeof(SolidColorBrush), typeof(SwitcherCheckBox), new PropertyMetadata(null));
        #endregion Property for switch border brush

        #region Property for switch border thickness
        public double SwitchBorderThinckness
        {
            get { return (double)GetValue(SwitchBorderThincknessProperty); }
            set { SetValue(SwitchBorderThincknessProperty, value); }
        }

        public static readonly DependencyProperty SwitchBorderThincknessProperty =
            DependencyProperty.Register(nameof(SwitchBorderThinckness), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for switch border thickness

        #region Property for switch size
        public double SwitchSize
        {
            get { return (double)GetValue(SwitchSizeProperty); }
            set { SetValue(SwitchSizeProperty, value); }
        }

        public static readonly DependencyProperty SwitchSizeProperty =
            DependencyProperty.Register(nameof(SwitchSize), typeof(double), typeof(SwitcherCheckBox), new PropertyMetadata(default(double)));
        #endregion Property for switch size

        #region Property for switch margin
        public Thickness SwitchMargin
        {
            get { return (Thickness)GetValue(SwitchMarginProperty); }
            set { SetValue(SwitchMarginProperty, value); }
        }

        public static readonly DependencyProperty SwitchMarginProperty =
            DependencyProperty.Register(nameof(SwitchMargin), typeof(Thickness), typeof(SwitcherCheckBox), new PropertyMetadata(null));
        #endregion Property for switch margin

        #region Property for text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SwitcherCheckBox), new PropertyMetadata(default(string)));
        #endregion Property for text

        #region Property for text visibility
        public Visibility TextVisibility
        {
            get { return (Visibility)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register(nameof(TextVisibility), typeof(Visibility), typeof(SwitcherCheckBox), new PropertyMetadata(default(Visibility)));
        #endregion Property for text visibility

        #region Property for text margin
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(SwitcherCheckBox), new PropertyMetadata(null));
        #endregion Property for text margin

        #endregion Public

        #endregion Properties

        #region Constructors
        static SwitcherCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitcherCheckBox), new FrameworkPropertyMetadata(typeof(SwitcherCheckBox)));
        }
        #endregion Constructors
    }
}
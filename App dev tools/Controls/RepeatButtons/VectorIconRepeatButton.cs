using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AppDevTools.Controls.RepeatButtons
{
    public class VectorIconRepeatButton : RepeatButton
    {
        #region Properties

        #region Public

        #region Property for vector icon
        public GeometryGroup VectorIcon
        {
            get { return (GeometryGroup)GetValue(VectorIconProperty); }
            set { SetValue(VectorIconProperty, value); }
        }

        public static readonly DependencyProperty VectorIconProperty =
            DependencyProperty.Register(nameof(VectorIcon), typeof(GeometryGroup), typeof(VectorIconRepeatButton), new PropertyMetadata(null));
        #endregion Property for vector icon

        #region Property for vector icon fill
        public SolidColorBrush VectorIconFill
        {
            get { return (SolidColorBrush)GetValue(VectorIconFillProperty); }
            set { SetValue(VectorIconFillProperty, value); }
        }

        public static readonly DependencyProperty VectorIconFillProperty =
            DependencyProperty.Register(nameof(VectorIconFill), typeof(SolidColorBrush), typeof(VectorIconRepeatButton), new PropertyMetadata(null));
        #endregion Property for vector icon fill

        #region Property for vector icon size
        public double VectorIconSize
        {
            get { return (double)GetValue(VectorIconSizeProperty); }
            set { SetValue(VectorIconSizeProperty, value); }
        }

        public static readonly DependencyProperty VectorIconSizeProperty =
            DependencyProperty.Register(nameof(VectorIconSize), typeof(double), typeof(VectorIconRepeatButton), new PropertyMetadata(default(double)));
        #endregion Property for vector icon size

        #region Property for vector icon stretch
        public Stretch VectorIconStretch
        {
            get { return (Stretch)GetValue(VectorIconStretchProperty); }
            set { SetValue(VectorIconStretchProperty, value); }
        }

        public static readonly DependencyProperty VectorIconStretchProperty =
           DependencyProperty.Register(nameof(VectorIconStretch), typeof(Stretch), typeof(VectorIconRepeatButton), new PropertyMetadata(default(Stretch)));
        #endregion Property for vector icon stretch

        #region Property for vector icon scale
        public double VectorIconScale
        {
            get { return (double)GetValue(VectorIconScaleProperty); }
            set { SetValue(VectorIconScaleProperty, value); }
        }

        public static readonly DependencyProperty VectorIconScaleProperty =
            DependencyProperty.Register(nameof(VectorIconScale), typeof(double), typeof(VectorIconRepeatButton), new PropertyMetadata(default(double)));
        #endregion Property for vector icon scale

        #region Property for vector icon margin
        public Thickness VectorIconMargin
        {
            get { return (Thickness)GetValue(VectorIconMarginProperty); }
            set { SetValue(VectorIconMarginProperty, value); }
        }

        public static readonly DependencyProperty VectorIconMarginProperty =
            DependencyProperty.Register(nameof(VectorIconMargin), typeof(Thickness), typeof(VectorIconRepeatButton), new PropertyMetadata(null));
        #endregion Property for icon margin

        #region Property for vector icon visibility
        public Visibility VectorIconVisibility
        {
            get { return (Visibility)GetValue(VectorIconVisibilityProperty); }
            set { SetValue(VectorIconVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VectorIconVisibilityProperty =
            DependencyProperty.Register(nameof(VectorIconVisibility), typeof(Visibility), typeof(VectorIconRepeatButton), new PropertyMetadata(default(Visibility)));
        #endregion Property for icon visibility

        #region Property for text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(VectorIconRepeatButton), new PropertyMetadata(default(string)));
        #endregion Property for text

        #region Property for text visibility
        public Visibility TextVisibility
        {
            get { return (Visibility)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register(nameof(TextVisibility), typeof(Visibility), typeof(VectorIconRepeatButton), new PropertyMetadata(default(Visibility)));
        #endregion Property for text visibility

        #region Property for text margin
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(VectorIconRepeatButton), new PropertyMetadata(null));
        #endregion Property for text margin

        #region Property for rounding corners
        public CornerRadius RoundingCorners
        {
            get { return (CornerRadius)GetValue(RoundingCornersProperty); }
            set { SetValue(RoundingCornersProperty, value); }
        }

        public static readonly DependencyProperty RoundingCornersProperty =
            DependencyProperty.Register(nameof(RoundingCorners), typeof(CornerRadius), typeof(VectorIconRepeatButton), new PropertyMetadata(new CornerRadius(0)));
        #endregion Property for rounding corners

        #endregion Public

        #endregion Properties

        #region Constructors
        static VectorIconRepeatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorIconRepeatButton), new FrameworkPropertyMetadata(typeof(VectorIconRepeatButton)));
        }
        #endregion Constructors
    }
}
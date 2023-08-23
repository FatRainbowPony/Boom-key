using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppDevTools.Controls.MenuItems
{
    public class VectorIconMenuItem : MenuItem
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
            DependencyProperty.Register(nameof(VectorIcon), typeof(GeometryGroup), typeof(VectorIconMenuItem), new PropertyMetadata(null));
        #endregion Property for vector icon

        #region Property for vector icon fill
        public SolidColorBrush VectorIconFill
        {
            get { return (SolidColorBrush)GetValue(VectorIconFillProperty); }
            set { SetValue(VectorIconFillProperty, value); }
        }

        public static readonly DependencyProperty VectorIconFillProperty =
            DependencyProperty.Register(nameof(VectorIconFill), typeof(SolidColorBrush), typeof(VectorIconMenuItem), new PropertyMetadata(null));
        #endregion Property for vector icon fill

        #region Property for vector icon size
        public double VectorIconSize
        {
            get { return (double)GetValue(VectorIconSizeProperty); }
            set { SetValue(VectorIconSizeProperty, value); }
        }

        public static readonly DependencyProperty VectorIconSizeProperty =
            DependencyProperty.Register(nameof(VectorIconSize), typeof(double), typeof(VectorIconMenuItem), new PropertyMetadata(default(double)));
        #endregion Property for vector icon size

        #region Property for vector icon stretch
        public Stretch VectorIconStretch
        {
            get { return (Stretch)GetValue(VectorIconStretchProperty); }
            set { SetValue(VectorIconStretchProperty, value); }
        }

        public static readonly DependencyProperty VectorIconStretchProperty =
           DependencyProperty.Register(nameof(VectorIconStretch), typeof(Stretch), typeof(VectorIconMenuItem), new PropertyMetadata(default(Stretch)));
        #endregion Property for vector icon stretch

        #region Property for vector icon scale
        public double VectorIconScale
        {
            get { return (double)GetValue(VectorIconScaleProperty); }
            set { SetValue(VectorIconScaleProperty, value); }
        }

        public static readonly DependencyProperty VectorIconScaleProperty =
            DependencyProperty.Register(nameof(VectorIconScale), typeof(double), typeof(VectorIconMenuItem), new PropertyMetadata(default(double)));
        #endregion Property for vector icon scale

        #region Property for vector icon margin
        public Thickness VectorIconMargin
        {
            get { return (Thickness)GetValue(VectorIconMarginProperty); }
            set { SetValue(VectorIconMarginProperty, value); }
        }

        public static readonly DependencyProperty VectorIconMarginProperty =
            DependencyProperty.Register(nameof(VectorIconMargin), typeof(Thickness), typeof(VectorIconMenuItem), new PropertyMetadata(null));
        #endregion Property for vector icon margin

        #region Property for vector icon visibility
        public Visibility VectorIconVisibility
        {
            get { return (Visibility)GetValue(VectorIconVisibilityProperty); }
            set { SetValue(VectorIconVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VectorIconVisibilityProperty =
            DependencyProperty.Register(nameof(VectorIconVisibility), typeof(Visibility), typeof(VectorIconMenuItem), new PropertyMetadata(default(Visibility)));
        #endregion Property for vector icon visibility

        #endregion Public

        #endregion Properties

        #region Constructors
        static VectorIconMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorIconMenuItem), new FrameworkPropertyMetadata(typeof(VectorIconMenuItem)));
        }
        #endregion Constructors
    }
}
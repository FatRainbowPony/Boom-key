using System.Windows;
using System.Windows.Controls.Primitives;

namespace AppDevTools.Controls.NumericUpDowns
{
    public class NumericUpDown : RangeBase
    {
        #region Properties

        #region Public

        #region Property for increment
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumericUpDown), new PropertyMetadata(default(double)));
        #endregion Property for increment

        #endregion Public

        #endregion Properties

        #region Constructor
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }
        #endregion Constructor
    }
}
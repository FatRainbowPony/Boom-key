using System.Windows;
using System.Windows.Controls;

namespace AppDevTools.Controls.TextBoxs
{
    public class HintTextBox : TextBox
    {
        #region Properties

        #region Public

        #region Property for hint
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(HintTextBox), new PropertyMetadata(default(string)));
        #endregion Propety for hint

        #endregion Public

        #endregion Properties

        #region Constructors
        static HintTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HintTextBox), new FrameworkPropertyMetadata(typeof(HintTextBox)));
        }
        #endregion Constructors
    }
}
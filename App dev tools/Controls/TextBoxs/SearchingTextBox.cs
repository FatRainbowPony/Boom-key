using System.Windows;
using System.Windows.Controls;

namespace AppDevTools.Controls.TextBoxs
{
    public class SearchingTextBox : TextBox
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
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(SearchingTextBox), new PropertyMetadata(default(string)));
        #endregion Propety for hint

        #region Property for determining the start of the search
        public bool IsStartedSearch
        {
            get { return (bool)GetValue(IsStartedSearchProperty); }
            set { SetValue(IsStartedSearchProperty, value); }
        }

        public static readonly DependencyProperty IsStartedSearchProperty =
            DependencyProperty.Register(nameof(IsStartedSearch), typeof(bool), typeof(SearchingTextBox), new PropertyMetadata(default(bool)));
        #endregion Property for determining the start of the search

        #region Property for determining the found element in the search 
        public bool IsFinded
        {
            get { return (bool)GetValue(IsFindedProperty); }
            set { SetValue(IsFindedProperty, value); }
        }

        public static readonly DependencyProperty IsFindedProperty =
            DependencyProperty.Register(nameof(IsFinded), typeof(bool), typeof(SearchingTextBox), new PropertyMetadata(default(bool)));
        #endregion Property for determining the found element in the search

        #endregion Public

        #endregion Properties

        #region Constructors
        static SearchingTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchingTextBox), new FrameworkPropertyMetadata(typeof(SearchingTextBox)));
        }
        #endregion Constructors
    }
}
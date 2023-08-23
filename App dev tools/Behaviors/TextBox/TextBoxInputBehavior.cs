using System;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.TextBox
{
    public class TextBoxInputBehavior : Behavior<sysControls.TextBox>
    {
        #region Constants

        #region Private
        private const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        #endregion Private

        #endregion Constants

        #region Enums

        #region Public
        public enum TextBoxInputMode
        {
            None,
            Decimal,
            DecimalInputWithoutMinus,
            DecimalInputForMaxValueDoubleAndWithoutMinus,
            DigitInput,
            DigitInputWithoutFirstZero,
            DigitInputForMaxValueInt32AndWithoutFirstZero,
            RangeByFormAnyNumbers
        }
        #endregion Public

        #endregion Enums

        #region Properties

        #region Public
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.Register(nameof(InputMode), typeof(TextBoxInputMode), typeof(TextBoxInputBehavior), new PropertyMetadata(null));

        public TextBoxInputMode InputMode
        {
            get { return (TextBoxInputMode)GetValue(InputModeProperty); }
            set { SetValue(InputModeProperty, value); }
        }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public TextBoxInputBehavior()
        {
            InputMode = TextBoxInputMode.None;
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!IsValidInput(GetText(" ")))
                {
                    SystemSounds.Beep.Play();
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsValidInput(GetText(e.Text)))
            {
                SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private void AssociatedObjectPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            if (textBox != null)
            {
                if (InputMode == TextBoxInputMode.Decimal ||
                    InputMode == TextBoxInputMode.DecimalInputWithoutMinus ||
                    InputMode == TextBoxInputMode.DecimalInputForMaxValueDoubleAndWithoutMinus)
                {
                    textBox.Text = GetDecimalNumberUpdated(textBox.Text);
                }

                if (InputMode == TextBoxInputMode.DigitInput ||
                    InputMode == TextBoxInputMode.DigitInputWithoutFirstZero ||
                    InputMode == TextBoxInputMode.DigitInputForMaxValueInt32AndWithoutFirstZero)
                {
                    textBox.Text = GetIntegerNumberUpdated(textBox.Text);
                }
            }

            string GetIntegerNumberUpdated(string str)
            {
                if (!string.IsNullOrEmpty(str) && str.ToCharArray()[0] == '0')
                {
                    str = str.Remove(0, 1);
                }

                return str;
            }

            string GetDecimalNumberUpdated(string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    foreach (char symbol in str.ToCharArray().ToList())
                    {
                        if (symbol == '.')
                        {
                            str = str.Replace('.', ',');
                        }
                    }

                    string[] substr = str.Split(',');

                    if (substr[0].ToCharArray().Where(x => x == '0').Count() > 1)
                    {
                        substr[0] = "0";
                    }

                    str = string.Join(",", substr);
                }

                return str;
            }
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                if (!IsValidInput(GetText(pastedText)))
                {
                    SystemSounds.Beep.Play();
                    e.CancelCommand();
                }
            }
            else
            {
                SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private string GetText(string input)
        {
            System.Windows.Controls.TextBox textBox = AssociatedObject;

            int selectionStart = textBox.SelectionStart;
            if (textBox.Text.Length < selectionStart)
            {
                selectionStart = textBox.Text.Length;
            }

            int selectionLength = textBox.SelectionLength;
            if (textBox.Text.Length < selectionStart + selectionLength)
            {
                selectionLength = textBox.Text.Length - selectionStart;
            }

            string realtext = textBox.Text.Remove(selectionStart, selectionLength);
            int caretIndex = textBox.CaretIndex;
            if (realtext.Length < caretIndex)
            {
                caretIndex = realtext.Length;
            }

            return realtext.Insert(caretIndex, input);
        }

        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;

                case TextBoxInputMode.DigitInput:
                    if (input.ToCharArray().Where(x => x == '0').Count() > 1)
                    {
                        return false;
                    }

                    return input.ToCharArray().All(char.IsDigit);

                case TextBoxInputMode.DigitInputWithoutFirstZero:
                    if (input.ToCharArray().First() == '0')
                    {
                        return false;
                    }

                    return input.ToCharArray().All(char.IsDigit);

                case TextBoxInputMode.DigitInputForMaxValueInt32AndWithoutFirstZero:
                    if (IsNumberGreaterThanMaxInt32(input) || input.ToCharArray().First() == '0')
                    {
                        return false;
                    }

                    return input.ToCharArray().All(char.IsDigit);

                case TextBoxInputMode.Decimal:
                    decimal d1;

                    if (input.ToCharArray().Where(x => x == ',').Count() > 1 || input.ToCharArray().Where(x => x == '-').Count() > 1)
                    {
                        return false;
                    }

                    if (input.ToCharArray().First() == '-' || input.ToCharArray().Where(x => x == '.').Count() == 1 && input.ToCharArray().Where(x => x == ',').Count() == 0)
                    {
                        return true;
                    }

                    return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d1);

                case TextBoxInputMode.DecimalInputWithoutMinus:
                    decimal d2;

                    if (input.ToCharArray().Where(x => x == ',').Count() > 1 || input.ToCharArray().Where(x => x == '-').Count() > 0)
                    {
                        return false;
                    }

                    if (input.ToCharArray().Where(x => x == '.').Count() == 1 && input.ToCharArray().Where(x => x == ',').Count() == 0)
                    {
                        return true;
                    }

                    return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d2);

                case TextBoxInputMode.DecimalInputForMaxValueDoubleAndWithoutMinus:
                    decimal d3;

                    if (input.ToCharArray().Where(x => x == ',').Count() > 1 || input.ToCharArray().Where(x => x == '-').Count() > 0 || IsNumberGreaterThanMaxDouble(input))
                    {
                        return false;
                    }

                    if (input.ToCharArray().Where(x => x == '.').Count() == 1 && input.ToCharArray().Where(x => x == ',').Count() == 0)
                    {
                        return true;
                    }

                    return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d3);

                case TextBoxInputMode.RangeByFormAnyNumbers:

                    if (input.ToCharArray().First() == ',')
                    {
                        return false;
                    }

                    var substringsWithoutEmptyStr = input.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                    for (int i = 0; i < substringsWithoutEmptyStr.Count - 1; i++)
                    {
                        if (GetTextBetween(input, substringsWithoutEmptyStr[i], substringsWithoutEmptyStr[i + 1]).ToCharArray().ToList().Count > 1)
                        {
                            return false;
                        }
                    }

                    var countEmptySubtring = (from substring in input.Split(",").ToList() where substring.Length == 0 select substring).Count();
                    if (countEmptySubtring > 1)
                    {
                        return false;
                    }

                    return IsRange(input);
            }

            bool IsNumberGreaterThanMaxInt32(string input)
            {
                try
                {
                    Convert.ToInt32(input);
                }
                catch (Exception exception)
                {
                    if (exception is OverflowException)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsNumberGreaterThanMaxDouble(string input)
            {
                try
                {
                    Convert.ToDouble(input);
                }
                catch (Exception exception)
                {
                    if (exception is OverflowException)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsRange(string input)
            {
                foreach (char symbol in input.ToCharArray())
                {
                    if ((symbol < '0' || symbol > '9') && symbol != ',')
                    {
                        return false;
                    }
                }

                return true;
            }

            string GetTextBetween(string source, string strStart, string strEnd)
            {
                if (source.Contains(strStart) && source.Contains(strEnd))
                {
                    int start, end;

                    start = source.IndexOf(strStart, 0) + strStart.Length;
                    end = source.IndexOf(strEnd, start);

                    return source[start..end];
                }

                return string.Empty;
            }

            return true;
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;
            AssociatedObject.PreviewLostKeyboardFocus += AssociatedObjectPreviewLostKeyboardFocus;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;
            AssociatedObject.PreviewLostKeyboardFocus -= AssociatedObjectPreviewLostKeyboardFocus;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }
        #endregion Protected

        #endregion Methods
    }
}
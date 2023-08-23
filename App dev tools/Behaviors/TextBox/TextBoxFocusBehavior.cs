using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.TextBox
{
    public class TextBoxFocusBehavior : Behavior<sysControls.TextBox>
    {
        #region Methods

        #region Private
        private void RemoveFocus()
        {
            DependencyObject scope = FocusManager.GetFocusScope(AssociatedObject);
            FrameworkElement parent = (FrameworkElement)AssociatedObject.Parent;

            while (parent != null && parent is IInputElement element && !element.Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            FocusManager.SetFocusedElement(scope, parent);
            Keyboard.ClearFocus();
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.LWin || e.Key == Key.RWin)
            {
                RemoveFocus();
            }
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyUp += KeyUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyUp -= KeyUp;
        }
        #endregion Protected

        #endregion Methods
    }
}
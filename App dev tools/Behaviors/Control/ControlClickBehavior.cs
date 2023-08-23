using System.Windows;
using System.Windows.Input;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.Control
{
    public class ControlClickBehavior
    {
        #region Properties

        #region Public
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ControlClickBehavior), new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ControlClickBehavior), new UIPropertyMetadata(null));
        #endregion Public

        #endregion Properties

        #region Methods

        #region Public
        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }
        #endregion Public

        #region Private
        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            sysControls.Control control = (sysControls.Control)target;
            if (control != null)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    control.PreviewMouseDown += PreviewMouseDown; ;
                }
                else if (e.NewValue == null && e.OldValue != null)
                {
                    control.PreviewMouseDown -= PreviewMouseDown;
                }
            }
        }

        private static void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sysControls.Control control = (sysControls.Control)sender;
            if (control != null)
            {
                ICommand command = (ICommand)control.GetValue(CommandProperty);
                object commandParameter = control.GetValue(CommandParameterProperty);
                command.Execute(commandParameter);
            }
        }
        #endregion Private

        #endregion Methods
    }
}
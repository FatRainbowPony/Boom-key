using System.Windows.Interactivity;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.ListBox
{
    public class ListBoxScrollIntoViewBehavior : Behavior<sysControls.ListBox>
    {
        #region Methods

        #region Private
        private void ScrollIntoView(object obj, sysControls.SelectionChangedEventArgs e)
        {
            sysControls.ListBox listBox = (sysControls.ListBox)obj;
            if (listBox == null)
            {
                return;
            }

            if (listBox.SelectedItem == null)
            {
                return;
            }

            sysControls.ListBoxItem listBoxItem = (sysControls.ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
            if (listBoxItem != null)
            {
                listBoxItem.BringIntoView();
            }
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += ScrollIntoView;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= ScrollIntoView;
            base.OnDetaching();
        }
        #endregion Protected

        #endregion Methods
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.ListBox
{
    public class ListBoxDragDropInOutsideAppBehavior : Behavior<sysControls.ListBox>
    {
        #region Fields

        #region Private
        private Point startPoint;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static readonly DependencyProperty PathsToDropFilesProperty =
            DependencyProperty.Register(nameof(PathsToDropFiles), typeof(List<string>), typeof(ListBoxDragDropInOutsideAppBehavior), new PropertyMetadata(null));

        public List<string> PathsToDropFiles
        {
            get { return (List<string>)GetValue(PathsToDropFilesProperty); }
            set { SetValue(PathsToDropFilesProperty, value); }
        }
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            sysControls.ListBox listBox = (sysControls.ListBox)sender;
            if (listBox != null)
            {
                Point mousePosition = e.GetPosition(null);
                Vector vector = startPoint - mousePosition;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance &&
                    Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (PathsToDropFiles == null || PathsToDropFiles.Count == 0)
                    {
                        return;
                    }

                    try
                    {
                        DragDrop.DoDragDrop(listBox, new DataObject(DataFormats.FileDrop, PathsToDropFiles.ToArray()), DragDropEffects.Copy);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove += MouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove -= MouseMove;
            base.OnDetaching();
        }
        #endregion Protected

        #endregion Methods
    }
}
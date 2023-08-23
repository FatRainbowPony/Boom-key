using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;
using AppDevTools.Addons;
using appDevToolsExts = AppDevTools.Extensions;
using sysWindows = System.Windows;

namespace AppDevTools.Behaviors.Window
{
    public class WindowSavingSizesBehavior : Behavior<sysWindows.Window>
    {
        #region Fields

        #region Private
        private Point location;
        private DispatcherTimer? searcherWindowTimer;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public Application CurrentApplication
        {
            get { return (Application)GetValue(CurrentApplicationProperty); }
            set { SetValue(CurrentApplicationProperty, value); }
        }

        public static readonly DependencyProperty CurrentApplicationProperty =
            DependencyProperty.Register(nameof(CurrentApplication), typeof(Application), typeof(WindowSavingSizesBehavior), new PropertyMetadata(null));
        
        public string PathToWindowInfo
        {
            get { return (string)GetValue(PathToWindowInfoProperty); }
            set { SetValue(PathToWindowInfoProperty, value); }
        }

        public static readonly DependencyProperty PathToWindowInfoProperty =
            DependencyProperty.Register(nameof(PathToWindowInfo), typeof(string), typeof(WindowSavingSizesBehavior), new PropertyMetadata(default(string)));
        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void Loaded(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            if (CurrentApplication == null)
            {
                return;
            }

            if (PathToWindowInfo == null)
            {
                return;
            }

            sysWindows.Window loadedWindow = (sysWindows.Window)sender;

            searcherWindowTimer = new() { Interval = TimeSpan.FromSeconds(1) };
            searcherWindowTimer.Tick += (sender, e) =>
            {
                sysWindows.Window? findedWindow = null;
                try
                {
                    findedWindow = appDevToolsExts.Window.Get(CurrentApplication, loadedWindow.Title);
                }
                catch { }
                if (findedWindow != null)
                {
                    if (findedWindow.WindowState != WindowState.Minimized)
                    {
                        location = findedWindow.PointToScreen(new Point(0, 0));
                    }

                    try
                    {
                        appDevToolsExts.File.Save(PathToWindowInfo, new List<WindowInfo> { new WindowInfo(location, new WindowInfo.SizeInfo(findedWindow.Height, findedWindow.Width), findedWindow.WindowState) });
                    }
                    catch { }
                }
            };

            if (File.Exists(PathToWindowInfo))
            {
                List<WindowInfo>? content = null;
                try
                {
                    content = appDevToolsExts.File.Load<WindowInfo>(PathToWindowInfo);
                }
                catch { }
                if (content != null)
                {
                    WindowInfo? windowInfo = content.FirstOrDefault();
                    if (windowInfo != null)
                    {
                        Point location = windowInfo.Location;
                        loadedWindow.Left = location.X;
                        loadedWindow.Top = location.Y;

                        WindowInfo.SizeInfo size = windowInfo.Size;
                        loadedWindow.Width = size.Width;
                        loadedWindow.Height = size.Height;

                        WindowState state = windowInfo.State;
                        switch (state)
                        {
                            case WindowState.Minimized:
                                loadedWindow.WindowState = WindowState.Normal;
                                break;

                            case WindowState.Maximized:
                                loadedWindow.WindowState = WindowState.Maximized;
                                break;
                        }
                    }
                }
            }

            searcherWindowTimer.Start();
        }

        private void Closing(object? sender, CancelEventArgs e)
        {
            searcherWindowTimer?.Stop();
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
            AssociatedObject.Closing += Closing;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= Loaded;
            AssociatedObject.Closing -= Closing;
            base.OnDetaching();
        }
        #endregion Protected

        #endregion Methods 
    }
}
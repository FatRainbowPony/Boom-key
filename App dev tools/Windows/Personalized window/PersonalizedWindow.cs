using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using appDevToolsExt = AppDevTools.Extensions;

namespace AppDevTools.Windows.PersonalizedWindow
{
    public static class Extensions
    {
        #region Methods

        #region Public
        public static void ExecuteActionFromChild(this object childDependencyObject, Action<PersonalizedWindow> action)
        {
            DependencyObject element = (DependencyObject)childDependencyObject;
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element);
                if (element is PersonalizedWindow window)
                {
                    action(window);
                    break;
                }
            }
        }

        public static void ExecuteActionFromTemplate(this object templateFrameworkElement, Action<PersonalizedWindow> action)
        {
            PersonalizedWindow window = (PersonalizedWindow)((FrameworkElement)templateFrameworkElement).TemplatedParent;
            if (window != null)
            {
                action(window);
            }
        }

        public static IntPtr GetHandle(this PersonalizedWindow window)
        {
            WindowInteropHelper windowInteropHelper = new(window);
            return windowInteropHelper.Handle;
        }
        #endregion Public

        #endregion Methods
    }

    public class PersonalizedWindow : appDevToolsExt.Window
    {
        #region Enums

        #region Private
        private enum AccentState
        {
            ACCENT_DISABLED,
            ACCENT_ENABLE_GRADIENT,
            ACCENT_ENABLE_TRANSPARENTGRADIENT,
            ACCENT_ENABLE_BLURBEHIND,
            ACCENT_INVALID_STATE,
        }

        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }
        #endregion Private

        #endregion Enums

        #region Structures

        #region Private
        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
        #endregion Private

        #endregion Structures

        #region Fields

        #region Private
        private Window? window;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public

        #region Property for rounding corners main body of window
        public CornerRadius WindowRoundingCorners
        {
            get { return (CornerRadius)GetValue(WindowRoundingCornersProperty); }
            set { SetValue(WindowRoundingCornersProperty, value); }
        }

        public static readonly DependencyProperty WindowRoundingCornersProperty =
            DependencyProperty.Register(nameof(WindowRoundingCorners), typeof(CornerRadius), typeof(PersonalizedWindow), new PropertyMetadata(new CornerRadius(0)));
        #endregion Property for rounding corners main body of window

        #region Properties for title bar

        #region Property for title bar background
        public SolidColorBrush TitleBarBackground
        {
            get { return (SolidColorBrush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register(nameof(TitleBarBackground), typeof(SolidColorBrush), typeof(PersonalizedWindow), new PropertyMetadata(null));
        #endregion Property for title bar background

        #region Property for rounding corners of title bar
        public CornerRadius TitleBarRoundingCorners
        {
            get { return (CornerRadius)GetValue(TitleBarRoundingCornersProperty); }
            set { SetValue(TitleBarRoundingCornersProperty, value); }
        }

        public static readonly DependencyProperty TitleBarRoundingCornersProperty =
            DependencyProperty.Register(nameof(TitleBarRoundingCorners), typeof(CornerRadius), typeof(PersonalizedWindow), new PropertyMetadata(new CornerRadius(0)));
        #endregion Property for rounding corners of title bar

        #region Property for title bar visibility
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TitleBarVisibilityProperty =
            DependencyProperty.Register(nameof(TitleBarVisibility), typeof(Visibility), typeof(PersonalizedWindow), new PropertyMetadata(default(Visibility)));
        #endregion Property for title bar visibility

        #endregion Properties for title bar

        #region Properties for caption

        #region Property for caption visibility
        public Visibility CaptionVisibility
        {
            get { return (Visibility)GetValue(CaptionVisibilityProperty); }
            set { SetValue(CaptionVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CaptionVisibilityProperty =
            DependencyProperty.Register(nameof(CaptionVisibility), typeof(Visibility), typeof(PersonalizedWindow), new PropertyMetadata(default(Visibility)));
        #endregion Property for caption visibility

        #region Property for caption foreground
        public SolidColorBrush CaptionForeground
        {
            get { return (SolidColorBrush)GetValue(CaptionForegroundProperty); }
            set { SetValue(CaptionForegroundProperty, value); }
        }

        public static readonly DependencyProperty CaptionForegroundProperty =
            DependencyProperty.Register(nameof(CaptionForeground), typeof(SolidColorBrush), typeof(PersonalizedWindow), new PropertyMetadata(null));
        #endregion Property for caption foreground

        #endregion Properties for caption

        #region Properties for managing button

        #region Property for width of managing button
        public double ManagingButtonWidth
        {
            get { return (double)GetValue(ManagingButtonWidthProperty); }
            set { SetValue(ManagingButtonWidthProperty, value); }
        }

        public static readonly DependencyProperty ManagingButtonWidthProperty =
            DependencyProperty.Register(nameof(ManagingButtonWidth), typeof(double), typeof(PersonalizedWindow), new PropertyMetadata(default(double)));
        #endregion Property for width of managing button

        #region Property for height of managing button
        public double ManagingButtonHeight
        {
            get { return (double)GetValue(ManagingButtonHeightProperty); }
            set { SetValue(ManagingButtonHeightProperty, value); }
        }

        public static readonly DependencyProperty ManagingButtonHeightProperty =
            DependencyProperty.Register(nameof(ManagingButtonHeight), typeof(double), typeof(PersonalizedWindow), new PropertyMetadata(default(double)));
        #endregion Property for height of managing button

        #region Property for rounding corners of managing button
        public CornerRadius ManagingButtonRoundingCorners
        {
            get { return (CornerRadius)GetValue(ManagingButtonRoundingCornersProperty); }
            set { SetValue(ManagingButtonRoundingCornersProperty, value); }
        }

        public static readonly DependencyProperty ManagingButtonRoundingCornersProperty =
            DependencyProperty.Register(nameof(ManagingButtonRoundingCorners), typeof(CornerRadius), typeof(PersonalizedWindow), new PropertyMetadata(new CornerRadius(0)));
        #endregion Property for rounding corners of managing button

        #region Property for visibility managing button
        public Visibility ManagingButtonVisibility
        {
            get { return (Visibility)GetValue(ManagingButtonVisibilityProperty); }
            set { SetValue(ManagingButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ManagingButtonVisibilityProperty =
            DependencyProperty.Register(nameof(ManagingButtonVisibility), typeof(Visibility), typeof(PersonalizedWindow), new PropertyMetadata(Visibility.Visible));
        #endregion Property for visibility managing button

        #endregion Properties for managing button

        #endregion Public

        #endregion Properties

        #region Constructors
        static PersonalizedWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PersonalizedWindow), new FrameworkPropertyMetadata(typeof(PersonalizedWindow)));        
        }
        #endregion Constructors
    }

    public partial class PersonalizedWindowActions
    {
        #region Constants

        #region Private
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_SIZE = 0xF000;
        private const int SC_KEYMENU = 0xF100;
        #endregion Private

        #endregion Constants

        #region Enums

        #region Private
        private enum ResizingAction
        {
            North = 3,
            South = 6,
            East = 2,
            West = 1,
            NorthEast = 5,
            NorthWest = 4,
            SouthEast = 8,
            SouthWest = 7
        }
        #endregion Private

        #endregion Enums

        #region Methods

        #region Private
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void ResizeNorth(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.North);
        }

        private void ResizeSouth(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.South);
        }

        private void ResizeEast(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.East);
        }

        private void ResizeWest(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.West);
        }

        private void ResizeNorthEast(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.NorthEast);
        }

        private void ResizeNorthWest(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.NorthWest);
        }

        private void ResizeSouthEast(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.SouthEast);
        }

        private void ResizeSouthWest(object sender, MouseButtonEventArgs e)
        {
            Resize(sender, ResizingAction.SouthWest);
        }

        private static void Resize(object sender, ResizingAction action)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                sender.ExecuteActionFromTemplate(window =>
                {
                    if (window.WindowState == WindowState.Normal)
                    {
                        DragResize(window.GetHandle(), action);
                    }
                });
            }

            static void DragResize(IntPtr handle, ResizingAction resizingAction)
            {
                SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + resizingAction), IntPtr.Zero);
                SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                sender.ExecuteActionFromTemplate(window => window.Close());
            }
            else
            {
                sender.ExecuteActionFromTemplate(window => SendMessage(window.GetHandle(), WM_SYSCOMMAND, (IntPtr)SC_KEYMENU, (IntPtr)' '));
            }
        }

        private void TitleBarMouseLeftButtonDownForResize(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                MaxButtonClick(sender, e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ExecuteActionFromTemplate(window => window.DragMove());
            }
        }

        private void TitleBarMouseLeftButtonDownForDialog(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                sender.ExecuteActionFromTemplate(window => window.WindowState = WindowState.Normal);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ExecuteActionFromTemplate(window => window.DragMove());
            }
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ExecuteActionFromTemplate(window =>
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        window.BeginInit();
                        double adjustment = 40.0;
                        Point mouse = e.MouseDevice.GetPosition(window);
                        double width1 = Math.Max(window.ActualWidth - 2 * adjustment, adjustment);
                        window.WindowState = WindowState.Normal;
                        double width2 = Math.Max(window.ActualWidth - 2 * adjustment, adjustment);
                        window.Left = (mouse.X - adjustment) * (1 - width2 / width1);
                        window.Top = -7;
                        window.EndInit();
                        window.DragMove();
                    }
                });
            }
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ExecuteActionFromTemplate(window => window.WindowState = WindowState.Minimized);
        }

        private void MaxButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ExecuteActionFromTemplate(window => window.WindowState = (window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized);
        }

        private void HideButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ExecuteActionFromTemplate(window => window.Hide());
        }

        private void HideHotkey(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.F4))
            {
                e.Handled = true;
                ((PersonalizedWindow)sender).Hide();
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ExecuteActionFromTemplate(window => window.Close());
        }
        #endregion Private

        #endregion Methods
    }

    public partial class PersonalizedWindowScaling
    {
        #region Constants

        #region Private 
        private const int DEF_MONITOR_TO_NEAREST = 0x00000002;
        #endregion Private

        #endregion Constants

        #region Enums

        #region Private
        private enum ABEdge
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        private enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }
        #endregion Private

        #endregion Enums

        #region Structures

        #region Private
        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public bool lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new();
            public RECT rcWork = new();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion Private

        #endregion Structures

        #region Fields

        #region Private
        private int height;
        private int width;
        #endregion Private

        #endregion Fields

        #region Methods

        #region Private 
        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private static MINMAXINFO AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, MINMAXINFO minMaxInfo)
        {
            IntPtr hWnd = FindWindow("Shell_TrayWnd", null);

            IntPtr monitorWithTaskbarOnIt = MonitorFromWindow(hWnd, DEF_MONITOR_TO_NEAREST);
            if (!monitorContainingApplication.Equals(monitorWithTaskbarOnIt))
            {
                return minMaxInfo;
            }

            APPBARDATA appBarData = new();
            appBarData.cbSize = Marshal.SizeOf(appBarData);
            appBarData.hWnd = hWnd;

            SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref appBarData);

            bool autoHide = Convert.ToBoolean(SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref appBarData));
            if (!autoHide)
            {
                return minMaxInfo;
            }

            int ABEdge = GetABEdge(appBarData.rc);
            switch (ABEdge)
            {
                case (int)PersonalizedWindowScaling.ABEdge.ABE_LEFT:
                    minMaxInfo.ptMaxPosition.x += 2;
                    minMaxInfo.ptMaxTrackSize.x -= 2;
                    minMaxInfo.ptMaxSize.x -= 2;
                    break;

                case (int)PersonalizedWindowScaling.ABEdge.ABE_RIGHT:
                    minMaxInfo.ptMaxSize.x -= 2;
                    minMaxInfo.ptMaxTrackSize.x -= 2;
                    break;

                case (int)PersonalizedWindowScaling.ABEdge.ABE_TOP:
                    minMaxInfo.ptMaxPosition.y += 2;
                    minMaxInfo.ptMaxTrackSize.y -= 2;
                    minMaxInfo.ptMaxSize.y -= 2;
                    break;

                case (int)PersonalizedWindowScaling.ABEdge.ABE_BOTTOM:
                    minMaxInfo.ptMaxSize.y -= 2;
                    minMaxInfo.ptMaxTrackSize.y -= 2;
                    break;

                default:
                    return minMaxInfo;
            }

            return minMaxInfo;

            [DllImport("user32", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

            [DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
            static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

            static int GetABEdge(RECT rect)
            {
                int ABEdge;

                if (rect.top == rect.left && rect.bottom > rect.right)
                {
                    ABEdge = (int)PersonalizedWindowScaling.ABEdge.ABE_LEFT;
                }
                else if (rect.top == rect.left && rect.bottom < rect.right)
                {
                    ABEdge = (int)PersonalizedWindowScaling.ABEdge.ABE_TOP;
                }
                else if (rect.top > rect.left)
                {
                    ABEdge = (int)PersonalizedWindowScaling.ABEdge.ABE_BOTTOM;
                }
                else
                {
                    ABEdge = (int)PersonalizedWindowScaling.ABEdge.ABE_RIGHT;
                }

                return ABEdge;
            }
        }

        private void GetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            object? obj = Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            if (obj != null)
            {
                MINMAXINFO minMaxInfo = (MINMAXINFO)obj;

                var monitorContainingApplication = MonitorFromWindow(hwnd, DEF_MONITOR_TO_NEAREST);
                if (monitorContainingApplication != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new();
                    GetMonitorInfo(monitorContainingApplication, monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    minMaxInfo.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                    minMaxInfo.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                    minMaxInfo.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                    minMaxInfo.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                    minMaxInfo.ptMaxTrackSize.x = minMaxInfo.ptMaxSize.x;
                    minMaxInfo.ptMaxTrackSize.y = minMaxInfo.ptMaxSize.y;
                    minMaxInfo.ptMinTrackSize.x = width;
                    minMaxInfo.ptMinTrackSize.y = height;
                    minMaxInfo = AdjustWorkingAreaForAutoHide(monitorContainingApplication, minMaxInfo);
                }

                Marshal.StructureToPtr(minMaxInfo, lParam, true);

                [DllImport("user32")]
                static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
            }
        }
        #endregion Private

        #region Public
        public void WindowInitialized(Window window, double height, double width)
        {
            this.height = Convert.ToInt32(height);
            this.width = Convert.ToInt32(width);
            HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).AddHook(new HwndSourceHook(WindowProc));

            IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == 0x0024)
                {
                    GetMinMaxInfo(hwnd, lParam);
                    handled = true;
                }

                return (IntPtr)0;
            }
        }
        #endregion Public

        #endregion Methods
    }
}
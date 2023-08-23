using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using sysWindows = System.Windows;

namespace AppDevTools.Extensions
{
    /// <summary>
    ///  Provides a collection of methods that extend the functionality of the System.Windows.Window class
    /// </summary>
    public class Window : sysWindows.Window
    {
        #region Enums

        #region Public
        public enum WindowStateWin32
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11
        }
        #endregion Public

        #endregion Enums

        #region Methods

        #region Private 
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int cmdShow);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);
        #endregion Private

        #region Public

        #region Method to check the window is open or not
        /// <summary>
        /// Checks window is open or not
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        /// <returns>
        /// true - if is opened, otherwise false
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsOpened(Application app, string titleWindow)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (titleWindow == null)
            {
                throw new ArgumentNullException(nameof(titleWindow));
            }

            List<sysWindows.Window> windows = app.Windows.Cast<sysWindows.Window>().Where(x => x.Title == titleWindow).ToList();
            if (windows.Count >= 1)
            {
                windows.First().Activate();

                return true;
            }

            return false;
        }
        #endregion Method to check the window is open or not

        #region Method for getting window
        /// <summary>
        /// Gets window of application
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        /// <returns>
        /// Window
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static sysWindows.Window? Get(Application app, string titleWindow)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (titleWindow == null)
            {
                throw new ArgumentNullException(nameof(titleWindow));
            }

            return app.Windows.Cast<sysWindows.Window>().Where(x => x.Title == titleWindow).ToList().FirstOrDefault();
        }
        #endregion Method for getting window

        #region Methods for activating window
        /// <summary>
        /// Activates window of application
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        public static void Activate(Application app, string titleWindow)
        {
            sysWindows.Window? window = Get(app, titleWindow);
            if (window != null)
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Activates window by the specified handle
        /// </summary>
        /// <param name="handleWindow">
        /// The specified handle for window
        /// </param>
        public static void Activate(IntPtr handleWindow)
        {
            SetForegroundWindow(handleWindow);
        }
        #endregion Methods for activating window

        #region Methods for showing window
        /// <summary>
        /// Shows window of application
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        public static void Show(Application app, string titleWindow)
        {
            sysWindows.Window? window = Get(app, titleWindow);
            if (window != null)
            {
                window.Activate();
                window.Show();
            }
        }

        /// <summary>
        /// Shows window by the specified handle
        /// </summary>
        /// <param name="handleWindow">
        /// The specified handle for window
        /// </param>
        /// <param name="windowState">
        /// The state of the window in which it will be shown
        /// </param>
        public static void Show(IntPtr handleWindow, WindowStateWin32 windowState)
        {
            ShowWindow(handleWindow, (int)windowState);
        }
        #endregion Methods for showing window

        #region Method for hiding window
        /// <summary>
        /// Hides window of application
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        public static void Hide(Application app, string titleWindow)
        {
            sysWindows.Window? window = Get(app, titleWindow);
            if (window != null)
            {
                window.Hide();
            }
        }
        #endregion Method for hiding window

        #region Method for closing window
        /// <summary>
        /// Closes window of application
        /// </summary>
        /// <param name="app">
        /// The application that the window belongs to
        /// </param>
        /// <param name="titleWindow">
        /// Window title
        /// </param>
        public static void Close(Application app, string titleWindow)
        {
            sysWindows.Window? window = Get(app, titleWindow);
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion Method for closing window

        #endregion Public

        #endregion Methods
    }
}
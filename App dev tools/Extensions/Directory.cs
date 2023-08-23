using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ookii.Dialogs.Wpf;
using sysIO = System.IO;

namespace AppDevTools.Extensions
{
    /// <summary>
    /// Provides a collection of methods that extend the functionality of the System.IO.Directory class
    /// </summary>
    public static class Directory
    {
        #region Structures

        #region Private
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        #endregion Private

        #endregion Structures

        #region Constants

        #region Private
        private const uint FILEICON = 0x100;
        private const uint LARGE_FILEICON = 0x0;
        #endregion Private

        #endregion Constants

        #region Methods

        #region Public

        #region Method to use folder browser dialog
        /// <summary>
        /// Opens folder browser dialog
        /// </summary>
        /// <param name="title">
        /// The title folder browser dialog
        /// </param>
        /// <returns>
        /// Selected path to folder
        /// </returns>
        public static string? UseBrowserDialog(string title)
        {
            const string DEF_TITLE = "Select folder";

            title ??= DEF_TITLE;

            VistaFolderBrowserDialog folderBrowserDialog = new() { UseDescriptionForTitle = true, Description = title };
            bool? resultDialog = folderBrowserDialog.ShowDialog();
            if (resultDialog == true)
            {
                if (!string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                {
                    return folderBrowserDialog.SelectedPath;
                }
            }

            return null;
        }
        #endregion Method to use folder browser dialog

        #region Method to show directory in explorer
        /// <summary>
        /// Shows directory in explorer
        /// </summary>
        /// <param name="pathToDir">
        /// Path to directory which will be showed
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.DirectoryNotFoundException"></exception>
        public static void ShowInExplorer(string pathToDir)
        {
            const string DEF_FILENAME = "Explorer";
            const string DEF_ARGS = "/e, /select, \"{0}\"";

            if (pathToDir == null)
            {
                throw new ArgumentNullException(nameof(pathToDir));
            }

            if (!sysIO.Directory.Exists(pathToDir))
            {
                throw new sysIO.DirectoryNotFoundException(nameof(pathToDir));
            }

            Process.Start(new ProcessStartInfo { FileName = DEF_FILENAME, Arguments = string.Format(DEF_ARGS, pathToDir) });
        }
        #endregion Method to show direcotry in explorer

        #region Method to create directory in AppData
        /// <summary>
        /// Creates directory in AppData
        /// </summary>
        /// <param name="dirName">
        /// Directory name which will be created
        /// </param>
        public static void CreateInAppData(string dirName)
        {
            string pathToDirInAppData = sysIO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName);
            sysIO.Directory.CreateDirectory(pathToDirInAppData);
        }
        #endregion Method to create directory in AppData

        #region Method to get path to directory in AppData
        /// <summary>
        /// Gets path to directory in AppData
        /// </summary>
        /// <param name="dirName">
        /// Direcotry name in AppData
        /// </param>
        /// <param name="permitCreate">
        /// Flag that indicates to allow the creation of a directory in AppData if it does not exist
        /// </param>
        /// <returns>
        /// Path to directory in AppData
        /// </returns>
        public static string? GetPathToDirInAppData(string dirName, bool permitCreate)
        {
            string pathToDirInAppData = sysIO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName);
            if (sysIO.Directory.Exists(pathToDirInAppData))
            {
                return pathToDirInAppData;
            }
            else if (permitCreate)
            {
                sysIO.Directory.CreateDirectory(pathToDirInAppData);

                return pathToDirInAppData;
            }

            return null;
        }
        #endregion Method to get path to directory in AppData

        #region Method to get directory icon
        /// <summary>
        /// Gets icon from directory
        /// </summary>
        /// <param name="pathToDir">
        /// Path to directory 
        /// </param>
        /// <returns>
        /// Directory icon
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.DirectoryNotFoundException"></exception>
        public static ImageSource GetIcon(string pathToDir)
        {
            if (pathToDir == null)
            {
                throw new ArgumentNullException(nameof(pathToDir));
            }

            if (!sysIO.Directory.Exists(pathToDir))
            {
                throw new sysIO.DirectoryNotFoundException();
            }

            SHFILEINFO fileInfo = new();
            SHGetFileInfo(pathToDir, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), FILEICON | LARGE_FILEICON);

            try
            {
                return Imaging.CreateBitmapSourceFromHIcon(Icon.FromHandle(fileInfo.hIcon).Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception)
            {
                throw;
            }

            [DllImport("shell32.dll")]
            static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }
        #endregion Method to get directory icon

        #endregion Public

        #endregion Methods
    }
}
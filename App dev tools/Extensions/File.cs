using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AppDevTools.Addons;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using sysIO = System.IO;

namespace AppDevTools.Extensions
{
    /// <summary>
    /// Provides a collection of methods that extend the functionality of the System.IO.File class
    /// </summary>
    public static class File
    {
        #region Methods

        #region Public

        #region Methods to use open file dialog

        #region Method to use open file dialog with single selection
        /// <summary>
        /// Opens file dialog with single selection
        /// </summary>
        /// <param name="title">
        /// The title open file dialog
        /// </param>
        /// <param name="filter">
        /// The filter files in open file dialog
        /// </param>
        /// <returns>
        /// Path to selected file
        /// </returns>
        public static string? UseOpenDialogSingleSel(string title, string filter)
        {
            const string DEF_TITLE = "Open file";
            const string DEF_FILTER = "All files (*.*)|*.*";

            title ??= DEF_TITLE;
            filter ??= DEF_FILTER;

            OpenFileDialog ofd = new()
            {
                Title = title,
                Filter = filter
            };
            bool? resultDialog = ofd.ShowDialog();
            if (resultDialog == true)
            {
                if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    return ofd.FileName;
                }
            }

            return null;
        }
        #endregion Method to use open file dialog with single selection

        #region Method to use open file dialog mult selection
        /// <summary>
        /// Opens file dialog with multiple selection
        /// </summary>
        /// <param name="title">
        /// The title open file dialog
        /// </param>
        /// <param name="filter">
        /// The filter files in open file dialog
        /// </param>
        /// <returns>
        /// List of paths to selected files
        /// </returns>
        public static List<string>? UseOpenDialogMultSel(string title, string filter)
        {
            const string DEF_TITLE = "Open files";
            const string DEF_FILTER = "All files (*.*)|*.*";

            title ??= DEF_TITLE;
            filter ??= DEF_FILTER;

            OpenFileDialog ofd = new()
            {
                Title = title,
                Filter = filter,
                Multiselect = true
            };
            bool? resultDialog = ofd.ShowDialog();
            if (resultDialog == true)
            {
                if (ofd.FileNames != null && ofd.FileNames.Length > 0)
                {
                    return ofd.FileNames.ToList();
                }
            }

            return null;
        }
        #endregion Method to use open file dialog mult selection

        #endregion Methods to use open file dialog

        #region Method to use save file dialog
        /// <summary>
        /// Opens save file dialog
        /// </summary>
        /// <param name="title">
        /// The title save file dialog
        /// </param>
        /// <param name="fileName">
        /// The file name in save file dialog
        /// </param>
        /// <param name="filter">
        /// The filter in save file dialog
        /// </param>
        /// <param name="ext">
        /// The extension in save file dialog
        /// </param>
        /// <returns>
        /// Path to saved file
        /// </returns>
        public static string? UseSaveDialog(string title, string fileName, string filter, string ext)
        {
            const string DEF_TITLE = "Save file";
            const string DEF_FILTER = "All files (*.*)|*.*";

            title ??= DEF_TITLE;
            fileName ??= string.Empty;
            filter ??= DEF_FILTER;
            ext ??= string.Empty;

            SaveFileDialog sfd = new()
            {
                Title = title,
                FileName = fileName,
                Filter = filter,
                DefaultExt = ext
            };
            bool? resultDialog = sfd.ShowDialog();
            if (resultDialog == true)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    return sfd.FileName;
                }
            }

            return null;
        }
        #endregion Method to use save file dialog

        #region Method to copy files recursively
        public static void CopyFilesRecursively(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in sysIO.Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                sysIO.Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in sysIO.Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                sysIO.File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }
        #endregion Method to copy files recursively

        #region Method to load file
        /// <summary>
        /// Loads file
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to load
        /// </typeparam>
        /// <param name="pathToLoad">
        /// Path to file for loading
        /// </param>
        /// <returns>
        /// The loaded file
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static List<T>? Load<T>(string pathToLoad)
        {
            if (pathToLoad == null)
            {
                throw new ArgumentNullException(nameof(pathToLoad));
            }

            if (!sysIO.File.Exists(pathToLoad))
            {
                throw new sysIO.FileNotFoundException();
            }

            string content;

            using (sysIO.StreamReader reader = new(pathToLoad))
            {
                content = reader.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
            {
                return Json.GetDeserializedObj<T>(content);
            }

            return null;
        }
        #endregion Method to load file

        #region Method to save file
        /// <summary>
        /// Saves file
        /// </summary>
        /// <param name="pathToSave">
        /// Path to save file
        /// </param>
        /// <param name="content">
        /// File contents
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Save(string pathToSave, object? content)
        {
            if (pathToSave == null)
            {
                throw new ArgumentNullException(nameof(pathToSave));
            }

            using (sysIO.StreamWriter writer = new(pathToSave, false))
            {
                writer.WriteLine(Json.GetSerializedObj(content));
            }
        }
        #endregion Method to save file

        #region Method to check file lock
        /// <summary>
        /// Checks file locking
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file which will be checked
        /// </param>
        /// <returns>
        /// true - if is locking, otherwise false
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static bool IsLocking(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!sysIO.File.Exists(pathToFile))
            {
                throw new sysIO.FileNotFoundException();
            }

            sysIO.FileStream fileStream;

            try
            {
                fileStream = new sysIO.FileStream(pathToFile, sysIO.FileMode.Open, sysIO.FileAccess.ReadWrite, sysIO.FileShare.None);
            }
            catch
            {
                return true;
            }

            if (fileStream != null)
            {
                fileStream.Dispose();
            }

            return false;
        }
        #endregion Method to check file lock

        #region Method to lock file
        /// <summary>
        /// Locks file
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file which will be locked
        /// </param>
        /// <returns>
        /// FileStream for locked file
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static sysIO.FileStream? Lock(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!sysIO.File.Exists(pathToFile))
            {
                throw new sysIO.FileNotFoundException();
            }

            try
            {
                return new sysIO.FileStream(pathToFile, sysIO.FileMode.Open, sysIO.FileAccess.ReadWrite, sysIO.FileShare.None);
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion Method to lock file

        #region Method to unlock file
        /// <summary>
        /// Unlocks file
        /// </summary>
        /// <param name="fileStream">
        /// FileStream for file whick will be unlocked
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Unlock(sysIO.FileStream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            fileStream.Close();
        }
        #endregion Method to unlock file

        #region Method to check identical files
        /// <summary>
        /// Checks indential files
        /// </summary>
        /// <param name="pathToFile1">
        /// Path to first file
        /// </param>
        /// <param name="pathToFile2">
        /// Path to second file
        /// </param>
        /// <returns>
        /// true - if are indential, otherwise false
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static bool AreIdentical(string pathToFile1, string pathToFile2)
        {
            const string DEF_DIRNAME = "temp";

            if (pathToFile1 == null)
            {
                throw new ArgumentNullException(nameof(pathToFile1));
            }

            if (!sysIO.File.Exists(pathToFile1))
            {
                throw new sysIO.FileNotFoundException();
            }

            if (pathToFile2 == null)
            {
                throw new ArgumentNullException(nameof(pathToFile2));
            }

            if (!sysIO.File.Exists(pathToFile2))
            {
                throw new sysIO.FileNotFoundException();
            }

            try
            {
                string pathToTempDir = Directory.GetPathToDirInAppData(sysIO.Path.Combine(DEF_DIRNAME, $"{DateTime.Now: dd.MM.yyyy}"), true)!;
                CopyInTempDir(pathToTempDir);

                sysIO.StreamReader streamReaderForFile1 = new(pathToFile1);
                sysIO.StreamReader streamReaderForFile2 = new(pathToFile2);

                while (!streamReaderForFile1.EndOfStream)
                {
                    if (streamReaderForFile2.EndOfStream)
                    {
                        StopStreamReading();

                        return false;
                    }

                    if (streamReaderForFile1.Read() != streamReaderForFile2.Read())
                    {
                        StopStreamReading();

                        return false;
                    }
                }

                StopStreamReading();

                void CopyInTempDir(string pathToTempDir)
                {
                    int countDirsInTempDir = sysIO.Directory.GetDirectories(pathToTempDir).ToList().Count;
                    int countFilesInTempDir = sysIO.Directory.GetFiles(pathToTempDir).ToList().Count;
                    if (countDirsInTempDir > 0 || countFilesInTempDir > 0)
                    {
                        sysIO.Directory.Delete(pathToTempDir, true);
                        sysIO.Directory.CreateDirectory(pathToTempDir);
                    }

                    string pathToDir1 = sysIO.Path.Combine(pathToTempDir, "0");
                    sysIO.Directory.CreateDirectory(pathToDir1);

                    string pathToDir2 = sysIO.Path.Combine(pathToTempDir, "1");
                    sysIO.Directory.CreateDirectory(pathToDir2);

                    string newPathToFile1 = sysIO.Path.Combine(pathToDir1, sysIO.Path.GetFileName(pathToFile1));
                    sysIO.File.Copy(pathToFile1, newPathToFile1);
                    pathToFile1 = newPathToFile1;

                    string newPathToFile2 = sysIO.Path.Combine(pathToDir2, sysIO.Path.GetFileName(pathToFile2));
                    sysIO.File.Copy(pathToFile2, newPathToFile2);
                    pathToFile2 = newPathToFile2;
                }

                void StopStreamReading()
                {
                    streamReaderForFile1.Dispose();
                    streamReaderForFile2.Dispose();

                    sysIO.Directory.Delete(pathToTempDir, true);
                }
            }
            catch 
            {
                return false;
            }

            return true;
        }
        #endregion Method to check identical files

        #region Method to show file in explorer
        /// <summary>
        /// Shows file in explorer
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file which will be showed
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static void ShowInExplorer(string pathToFile)
        {
            const string DEF_FILENAME = "Explorer";
            const string DEF_ARGS = "/e, /select, \"{0}\"";

            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!sysIO.File.Exists(pathToFile))
            {
                throw new sysIO.FileNotFoundException();
            }

            Process.Start(new ProcessStartInfo { FileName = DEF_FILENAME, Arguments = string.Format(DEF_ARGS, pathToFile) });
        }
        #endregion Method to show file in explorer

        #region Methods to get file icon
        /// <summary>
        /// Gets file icon
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file
        /// </param>
        /// <returns>
        /// File icon
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="sysIO.FileNotFoundException"></exception>
        public static ImageSource? GetIcon(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!sysIO.File.Exists(pathToFile))
            {
                throw new sysIO.FileNotFoundException();
            }

            ImageSource? imageSource = null;
            using (Icon? sysIcon = Icon.ExtractAssociatedIcon(pathToFile))
            {
                if (sysIcon != null)
                {
                    try
                    {
                        imageSource = Imaging.CreateBitmapSourceFromHIcon(sysIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return imageSource;
        }
        #endregion Methods to get file icon

        #region Method to create URL-file
        /// <summary>
        /// Creates URL-file
        /// </summary>
        /// <param name="pathToCreate">
        /// The path where the URL file will be created
        /// </param>
        /// <param name="url">
        /// The URL to be placed in the file
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CreateUrl(string pathToCreate, string url)
        {
            const string DEF_TAG = "InternetShortcut";
            const string DEF_TYPE = "URL";

            if (pathToCreate == null)
            {
                throw new ArgumentNullException(nameof(pathToCreate));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            sysIO.File.WriteAllText(pathToCreate, $"[{DEF_TAG}]\r\n{DEF_TYPE}={url}");
        }
        #endregion Method to create URL-file

        #region Method to create shortcut file
        /// <summary>
        /// Creates shortcut file
        /// </summary>
        /// <param name="pathToCreate">
        /// The path where the shortcut file will be created
        /// </param>
        /// <param name="targetPath">
        /// The path to the target file or folder to be placed in the shortcut file
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CreateShortcut(string pathToCreate, string targetPath)
        {
            if (pathToCreate == null)
            {
                throw new ArgumentNullException(nameof(pathToCreate));
            }

            if (targetPath == null)
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            if (sysIO.File.Exists(pathToCreate))
            {
                sysIO.File.Delete(pathToCreate);
            }

            IWshShortcut shortcut = (IWshShortcut)new WshShell().CreateShortcut(pathToCreate);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }
        #endregion Method to create shortcut file

        #endregion Public

        #endregion Methods
    }
}
using System;
using Microsoft.Win32;

namespace AppDevTools.Addons
{
    /// <summary>
    /// Provides a collection of properties and methods for registering application autorun
    /// </summary>
    public class Autorun
    {
        #region Constants

        #region Private
        private const string DEF_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        #endregion Private

        #endregion Constants

        #region Properties

        #region Public
        /// <summary>
        /// Gets the specific register key
        /// </summary>
        public RegistryKey? RegistryKey { get; private set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        /// <summary>
        /// Initializes a new instance of the Autorun class
        /// </summary>
        public Autorun()
        {
            RegistryKey = Registry.CurrentUser.OpenSubKey(DEF_KEY, true);
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Public

        #region Method to get autorun registration
        /// <summary>
        /// Gets autorun registration associated with the specified name.
        /// Returns null if the name/value doesn't exist in the registry
        /// </summary>
        /// <param name="appName">
        /// Application name
        /// </param>
        /// <returns>
        /// The value associated with name, or null if name is not found
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public object? Get(string? appName)
        {
            if (RegistryKey == null)
            {
                throw new ArgumentNullException(nameof(RegistryKey));
            }

            return RegistryKey.GetValue(appName);
        }
        #endregion Method to get autorun registration

        #region Method to set autorun registration
        /// <summary>
        /// Sets autorun registration
        /// </summary>
        /// <param name="appName">
        /// Application name
        /// </param>
        /// <param name="executablePathApp">
        /// Path to the application executable file
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(string? appName, string executablePathApp)
        {
            if (RegistryKey == null)
            {
                throw new ArgumentNullException(nameof(RegistryKey));
            }

            if (executablePathApp == null)
            {
                throw new ArgumentNullException(nameof(executablePathApp));
            }

            RegistryKey.SetValue(appName, executablePathApp);
        }
        #endregion Method to set autorun registration

        #region Method to delete autorun registration
        /// <summary>
        /// Deletes autorun registration
        /// </summary>
        /// <param name="appName">
        /// Application name
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Delete(string appName)
        {
            if (RegistryKey == null)
            {
                throw new ArgumentNullException(nameof(RegistryKey));
            }

            if (appName == null)
            {
                throw new ArgumentNullException(nameof(appName));
            }

            RegistryKey.DeleteValue(appName);
        }
        #endregion Method to delete autorun registration

        #endregion Public

        #endregion Methods
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppDevTools.Generators
{
    /// <summary>
    /// Provides a collection of methods for generating name
    /// </summary>
    public static class NameGenerator
    {
        #region Methods

        #region Public
        /// <summary>
        /// Generates a unique name by adding a digit at the end of the name
        /// </summary>
        /// <param name="originalName">
        /// Original name
        /// </param>
        /// <param name="existingNames">
        /// List of existing names
        /// </param>
        /// <returns>
        /// New generated name
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Start(string originalName, List<string> existingNames)
        {
            if (originalName == null)
            {
                throw new ArgumentNullException(nameof(originalName));
            }

            if (existingNames == null)
            {
                throw new ArgumentNullException(nameof(existingNames));
            }

            int num = 1;
            string newName = originalName;

            while (IsExistingSameName(newName))
            {
                newName = $"{originalName} {num++}";
            }

            return newName;

            bool IsExistingSameName(string name)
            {
                int countExistingNames = (from existingName in existingNames
                                          where existingName == name
                                          select existingName).Count();
                if (countExistingNames > 0)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion Public

        #endregion Methods
    }
}
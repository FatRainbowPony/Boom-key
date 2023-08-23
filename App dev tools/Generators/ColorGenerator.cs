using System;
using System.Windows.Media;

namespace AppDevTools.Generators
{
    /// <summary>
    /// Provides a collection of methods for generating color
    /// </summary>
    public class ColorGenerator
    {
        #region Fields

        #region Private
        private readonly Random random;
        #endregion Private

        #endregion Fields

        #region Constructors

        #region Public
        /// <summary>
        /// Initializes a new instance of the ColorGenerator class
        /// </summary>
        public ColorGenerator()
        {
            random = new Random();
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Public
        /// <summary>
        /// Generates random color
        /// </summary>
        /// <returns>
        /// Random generated color
        /// </returns>
        public Color Start()
        {
            return Color.FromArgb(100, (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
        }
        #endregion Public

        #endregion Methods
    }
}
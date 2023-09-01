using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppDevTools.Addons
{
    /// <summary>
    /// Provides a collection of properties and methods for managing language for application
    /// </summary>
    public class Lang
    {
        #region Enums

        #region Public
        /// <summary>
        /// The specific description that describe the language
        /// </summary>
        public enum LangDescription
        {
            Rus,
            Eng
        }
        #endregion Public

        #endregion Enums

        #region Properties

        #region Public
        /// <summary>
        /// Gets or sets the language description
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LangDescription Description { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        /// <summary>
        /// Initializes a new instance of the Lang class default
        /// </summary>
        public Lang()
        {
            Description = LangDescription.Rus;
        }

        /// <summary>
        /// Initializes a new instance of the Lang class with the specific language description
        /// </summary>
        /// <param name="description">
        /// The language description
        /// </param>
        public Lang(LangDescription description)
        {
            Description = description;
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Public
        /// <summary>
        /// Gets language from file
        /// </summary>
        /// <param name="pathToFile">
        /// Path to file
        /// </param>
        /// <returns>
        /// The language, or null if the file does't contain data about the language
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Lang? Get(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException(nameof(pathToFile));
            }

            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException();
            }

            List<Lang>? content = Json.GetDeserializedObj<Lang>(File.ReadAllText(pathToFile));
            if (content != null)
            {
                return content.FirstOrDefault();
            }

            return null;
        }
        #endregion Public

        #endregion Methods
    }
}
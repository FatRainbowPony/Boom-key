using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppDevTools.Addons
{
    /// <summary>
    /// Provides a collection of methods for converting between .NET types and JSON types
    /// </summary>
    public static class Json
    {
        #region Methods

        #region Public

        #region Method to get serialized object
        /// <summary>
        /// Serializes the specified object to a JSON string
        /// </summary>
        /// <param name="value">
        /// The object to serialize
        /// </param>
        /// <returns>
        /// A JSON string representation of the object
        /// </returns>
        public static string GetSerializedObj(object? obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion Method to get serialized object

        #region Method to get deserialized object
        /// <summary>
        /// Deserializes the JSON to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to deserialize
        /// </typeparam>
        /// <param name="value">
        /// The JSON to deserialize
        /// </param>
        /// <returns>
        /// The deserialized object from the JSON string
        /// </returns>
        public static List<T>? GetDeserializedObj<T>(string str)
        {
            if (str == null)
            {
                return new List<T>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(str);
            }
            catch
            {
                return new List<T>();
            }
        }
        #endregion Method to get deserialized object

        #endregion Public

        #endregion Methods
    }
}
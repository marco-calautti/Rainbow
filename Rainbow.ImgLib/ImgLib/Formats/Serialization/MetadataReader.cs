using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public interface MetadataReader : IDisposable
    {
        /// <summary>
        /// Enters the given section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="name"></param>
        void EnterSection(string name);

        /// <summary>
        /// Exits the given section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        void ExitSection();

        /// <summary>
        /// Retrieves the string value of the given key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);

        /// <summary>
        /// Retrieves the int value of the given key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetInt(string key);

        /// <summary>
        /// Retrieves the long value of the given key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        long GetLong(string key);

        /// <summary>
        /// Retrieves the raw byte array value of the given key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetRaw(string key);

        /// <summary>
        /// Retrieves the bool value of the given key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetBool(string key);

        /// <summary>
        /// Retrieves the string value of the given attribute key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetAttributeString(string key);

        /// <summary>
        /// Retrieves the int value of the given attribute key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetAttributeInt(string key);

        /// <summary>
        /// Retrieves the long value of the given attribute key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        long GetAttributeLong(string key);

        /// <summary>
        /// Retrieves the bool value of the given attribute key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetAttributeBool(string key);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization
{
    /// <summary>
    /// Interface exposing methods to store metadata informations for exporting Texture Formats.
    /// </summary>
    public interface MetadataWriter : IDisposable
    {
        /// <summary>
        /// Creates a new section with the given name. Many sections may be nested until the corresponding EndSection is not called.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="name"></param>
        void BeginSection(string name);

        /// <summary>
        /// Closes the currently create section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        void EndSection();

        /// <summary>
        /// Adds the given string associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, string value);

        /// <summary>
        /// Adds the given int associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, int value);

        /// <summary>
        /// Adds the given long associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, long value);

        /// <summary>
        /// Adds the given array of bytes associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, byte[] value);

        /// <summary>
        /// Adds the given bool associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, bool value);

        /// <summary>
        /// Adds the given string associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, string value);

        /// <summary>
        /// Adds the given int associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, int value);

        /// <summary>
        /// Adds the given long associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, long value);

        /// <summary>
        /// Adds the given bool associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, string bool);
    }
}

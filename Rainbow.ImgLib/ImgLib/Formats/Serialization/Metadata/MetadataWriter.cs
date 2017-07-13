//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization.Metadata
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
        /// Adds the given byte associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, byte value);

        /// <summary>
        /// Adds the given short associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, short value);

        /// <summary>
        /// Adds the given ushort associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, ushort value);

        /// <summary>
        /// Adds the given int associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, int value);

        /// <summary>
        /// Adds the given uint associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, uint value);

        /// <summary>
        /// Adds the given long associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, long value);

        /// <summary>
        /// Adds the given ulong associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, ulong value);

        /// <summary>
        /// Adds the given float associated to the given key into the current section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, float value);

        /// <summary>
        /// Adds the given double associated to the given key into the current section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, double value);

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
        /// Adds the given byte associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, byte value);

        /// <summary>
        /// Adds the given short associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, short value);

        /// <summary>
        /// Adds the given ushort associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, ushort value);

        /// <summary>
        /// Adds the given int associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, int value);

        /// <summary>
        /// Adds the given uint associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, uint value);

        /// <summary>
        /// Adds the given long associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, long value);

        /// <summary>
        /// Adds the given ulong associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, ulong value);

        /// <summary>
        /// Adds the given bool associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, bool value);

        /// <summary>
        /// Adds the given float associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, float value);

        /// <summary>
        /// Adds the given double associated to the given key as a special section attribute.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PutAttribute(string key, double value);
    }
}

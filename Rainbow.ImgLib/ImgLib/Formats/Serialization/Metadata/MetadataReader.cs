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
    /// Interface for readding textures metadata informations.
    /// </summary>
    public interface MetadataReader : IDisposable
    {
        /// <summary>
        /// Enters the given section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="name"></param>
        void EnterSection(string name);

        /// <summary>
        /// Exits the given section. Once the section is exited, it cannot be entered.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        void ExitSection();

        /// <summary>
        /// Retrieves the value of type T for the given key. Supported types are all the once
        /// supported by writing methods of MetadataWriters.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Retrieves the value of type T for the given attribute key:
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetAttribute<T>(string key);

        /// <summary>
        /// Rewinds this MetadataReader from the very beginning, allowing to traverse it again.
        /// </summary>
        void Rewind();

        /// <summary>
        /// Retrieves all the keys for data values in the currently entered section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        ICollection<string> Keys { get; }

        /// <summary>
        /// Retrivies all the keys for the attribute values in the currently entered section.
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        ICollection<string> AttributesKeys { get; }

        
        /// <summary>
        /// Returns the C# type of the data value with key "key".
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        Type GetValueType(string key);

        /// <summary>
        /// Returns the C# type of the attribute value with key "key".
        /// </summary>
        /// <exception cref="MetadataException"></exception>
        /// <param name="key"></param>
        /// <returns></returns>
        Type GetAttributeValueType(string key);
    }
}

//Copyright (C) 2014 Marco (Phoenix) Calautti.

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

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

using Rainbow.ImgLib.Formats.Serialization.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization.Metadata
{
    public abstract class MetadataReaderBase : MetadataReader
    {
        public abstract void EnterSection(string name);

        public abstract void ExitSection();

        public abstract void Rewind();

        public abstract ICollection<string> Keys { get; }

        public abstract ICollection<string> AttributesKeys { get; }

        public abstract Type GetValueType(string key);

        public abstract Type GetAttributeValueType(string key);

        protected abstract string GetDataStringRepresentation(string key);

        protected abstract string GetAttributeStringRepresentation(string key);

        public T Get<T>(string key)
        {
            return _Get<T>(key, GetDataStringRepresentation(key), GetValueType(key));
        }

        public T GetAttribute<T>(string key)
        {
            return _Get<T>(key, GetAttributeStringRepresentation(key), GetAttributeValueType(key));
        }

        private T _Get<T>(string key, string representation, Type t)
        {
            Type myType = typeof(T);
            if (t != myType)
                throw new MetadataException("The value/attribute with key " + key + " is not a of type " + myType.Name + "!");

            object ret = null;
            try
            {
                if (myType == typeof(byte[]))
                {
                    ret = Convert.FromBase64String(representation);

                }
                else if (myType == typeof(string))
                {
                    ret = representation;
                }
                else if (myType == typeof(byte))
                {
                    ret = byte.Parse(representation);
                }
                else if (myType == typeof(short))
                {
                    ret = short.Parse(representation);
                }
                else if (myType == typeof(ushort))
                {
                    ret = ushort.Parse(representation);
                }
                else if (myType == typeof(int))
                {
                    ret = int.Parse(representation);
                }
                else if (myType == typeof(uint))
                {
                    ret = uint.Parse(representation);
                }
                else if (myType == typeof(long))
                {
                    ret = long.Parse(representation);
                }
                else if (myType == typeof(ulong))
                {
                    ret = ulong.Parse(representation);
                }
                else if (myType == typeof(float))
                {
                    ret = float.Parse(representation);
                }
                else if (myType == typeof(double))
                {
                    ret = double.Parse(representation);
                }
                else if (myType == typeof(bool))
                {
                    ret = bool.Parse(representation);
                }
                else
                {
                    throw new MetadataException("Unsupported type " + myType.Name + "!");
                }
            }
            catch (Exception e)
            {
                throw new MetadataException("Cannot get element value " + key + "!", e);
            }

            return (T)ret;
        }

        public abstract void Dispose();
    }
}

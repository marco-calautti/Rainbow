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

using Rainbow.ImgLib.Formats.Serialization.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public static class InteropUtils
    {
        public static void WriteTo(GenericDictionary dictionary, MetadataWriter metadata)
        {
            foreach(string key in dictionary.Keys)
            {
                object value = dictionary.Get<object>(key);
                metadata.GetType().InvokeMember("Put", BindingFlags.InvokeMethod, Type.DefaultBinder, metadata, new object[]{ key, value });
            }
        }

        public static void ReadIntFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<int>(key, metadata.GetInt(key));
        }

        public static void ReadLongFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<long>(key, metadata.GetLong(key));
        }

        public static void ReadStringFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<string>(key, metadata.GetString(key));
        }

        public static void ReadRawFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<byte[]>(key, metadata.GetRaw(key));
        }

        public static void ReadBoolFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<bool>(key, metadata.GetBool(key));
        }

        public static void ReadByteFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<byte>(key, (byte)metadata.GetInt(key));
        }

        public static void ReadSByteFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<sbyte>(key, (sbyte)metadata.GetInt(key));
        }

        public static void ReadShortFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<short>(key, (short)metadata.GetInt(key));
        }

        public static void ReadUShortFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<ushort>(key, (ushort)metadata.GetInt(key));
        }

        public static void ReadUIntFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<uint>(key, (uint)metadata.GetInt(key));
        }

        public static void ReadULongFrom(MetadataReader metadata, GenericDictionary dictionary, string key)
        {
            dictionary.Put<ulong>(key, (ulong)metadata.GetLong(key));
        }
    }
}

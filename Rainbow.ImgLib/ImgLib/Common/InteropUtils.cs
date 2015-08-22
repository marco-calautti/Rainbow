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

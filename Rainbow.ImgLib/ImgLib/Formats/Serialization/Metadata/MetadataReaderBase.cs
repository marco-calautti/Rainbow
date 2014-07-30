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

        public abstract string GetString(string key);

        public int GetInt(string key)
        {
            try
            {
                return int.Parse(GetString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get element value "+key+"!",e);
            }
        }

        public long GetLong(string key)
        {
            try
            {
                return long.Parse(GetString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get element value "+key+"!",e);
            }
        }

        public byte[] GetRaw(string key)
        {
            try
            {
                return Convert.FromBase64String(GetString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get element value "+key+"!",e);
            }
        }

        public bool GetBool(string key)
        {
            try
            {
                return bool.Parse(GetString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get element value "+key+"!",e);
            }
        }

        public abstract string GetAttributeString(string key);

        public int GetAttributeInt(string key)
        {
            try
            {
                return int.Parse(GetAttributeString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get attribute value "+key+"!",e);
            }
        }

        public long GetAttributeLong(string key)
        {
            try
            {
                return long.Parse(GetAttributeString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get attribute value "+key+"!",e);
            }
        }

        public bool GetAttributeBool(string key)
        {
            try
            {
                return bool.Parse(GetAttributeString(key));
            }catch(Exception e)
            {
                throw new MetadataException("Cannot get attribute value "+key+"!",e);
            }
        }

        public abstract void Dispose();

    }
}

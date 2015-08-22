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

        public abstract string GetString(string key);

        public abstract void Rewind();

        public abstract IEnumerable<string> Keys();

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

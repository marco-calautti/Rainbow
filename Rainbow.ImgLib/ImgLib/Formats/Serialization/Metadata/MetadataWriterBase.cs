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
    public abstract class MetadataWriterBase : MetadataWriter
    {
        public abstract void BeginSection(string name);

        public abstract void EndSection();

        protected abstract void PutWithType(string key, string value, Type type);

        public void Put(string key, string value)
        {
            PutWithType(key, value, value.GetType());
        }

        public void Put(string key, byte value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, short value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, ushort value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, int value)
        {
            PutWithType(key, value.ToString(),value.GetType());
        }

        public void Put(string key, uint value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, long value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, ulong value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, byte[] value)
        {
            PutWithType(key, Convert.ToBase64String(value) , value.GetType());
        }

        public void Put(string key, bool value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, float value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        public void Put(string key, double value)
        {
            PutWithType(key, value.ToString(), value.GetType());
        }

        protected abstract void PutAttributeWithType(string key, string value, Type type);

        public void PutAttribute(string key, string value)
        {
            PutAttributeWithType(key, value, value.GetType());
        }

        public void PutAttribute(string key, byte value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, short value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, ushort value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, int value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, uint value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, long value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, ulong value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, bool value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, float value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public void PutAttribute(string key, double value)
        {
            PutAttributeWithType(key, value.ToString(), value.GetType());
        }

        public abstract void Dispose();
    }
}

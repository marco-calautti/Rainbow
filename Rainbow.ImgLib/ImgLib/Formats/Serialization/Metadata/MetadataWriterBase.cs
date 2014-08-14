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
    public abstract class MetadataWriterBase : MetadataWriter
    {
        public abstract void BeginSection(string name);

        public abstract void EndSection();

        public abstract void Put(string key, string value);

        public void Put(string key, int value)
        {
            Put(key, value.ToString());
        }

        public void Put(string key, long value)
        {
            Put(key, value.ToString());
        }

        public void Put(string key, byte[] value)
        {
            Put(key, Convert.ToBase64String(value));
        }

        public void Put(string key, bool value)
        {
            Put(key, value.ToString());
        }

        public abstract void PutAttribute(string key, string value);

        public void PutAttribute(string key, int value)
        {
            PutAttribute(key, value.ToString());
        }

        public void PutAttribute(string key, long value)
        {
            PutAttribute(key, value.ToString());
        }

        public void PutAttribute(string key, bool value)
        {
            PutAttribute(key, value.ToString());
        }
        public abstract void Dispose();
    }
}

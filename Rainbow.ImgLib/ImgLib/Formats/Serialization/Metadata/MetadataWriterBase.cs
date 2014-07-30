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

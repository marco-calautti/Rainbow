using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization.Exceptions
{
    public class MetadataException : Exception
    {
        public MetadataException() { }
        public MetadataException(string message) : base(message) { }
        public MetadataException(string message, Exception inner) : base(message, inner) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public interface EndiannessDependent
    {
        ByteOrder ByteOrder { get; set; }
    }
}

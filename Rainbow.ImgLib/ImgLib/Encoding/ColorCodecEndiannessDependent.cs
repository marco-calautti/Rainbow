using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding
{
    public abstract class ColorCodecEndiannessDependent : ColorCodec, EndiannessDependent
    {
        private ByteOrder order;
        public ColorCodecEndiannessDependent(ByteOrder order)
        {
            this.order = order;
        }

        public ByteOrder ByteOrder { get { return order; } set { order = value; } }
    }
}

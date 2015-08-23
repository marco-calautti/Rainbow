using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecDXT1Gamecube : ColorCodecDXT1
    {
        private ImageFilter filter;

        public ColorCodecDXT1Gamecube(int width, int height) :
            base(ByteOrder.BigEndian, width, height) 
        {
            filter = new TileFilter(64, 2, 2, width / 4, height / 4);
        }

        protected override Filters.ImageFilter GetImageFilter()
        {
            return filter;
        }
    }
}

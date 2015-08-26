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
            filter = new TileFilter(64, 2, 2, FullWidth / 4, FullHeight / 4);
        }

        protected override Filters.ImageFilter GetImageFilter()
        {
            return filter;
        }

        public override int FullWidth
        {
            get
            {
                int oldWidth=base.FullWidth;
                return oldWidth % 8 != 0 ? (oldWidth/8)*8 + 8 : oldWidth;
            }
        }

        public override int FullHeight
        {
            get
            {
                int oldHeight = base.FullHeight;
                return oldHeight % 8 != 0 ? (oldHeight/8)*8 + 8  : oldHeight;
            }
        }
    }
}

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
        private readonly ImageFilter filter;

        public ColorCodecDXT1Gamecube(int width, int height) :
            base(ByteOrder.BigEndian, width, height) 
        {
            filter = new TileFilter(64, 2, 2, FullWidth / 4, FullHeight / 4);
        }

        protected override Filters.ImageFilter GetImageFilter()
        {
            return filter;
        }

        protected override int GetFullWidth(int width)
        {
            int oldWidth=base.GetFullWidth(width);
            return oldWidth % 8 != 0 ? (oldWidth/8)*8 + 8 : oldWidth;
        }

        protected override int GetFullHeight(int height)
        {
            int oldHeight = base.GetFullHeight(height);
            return oldHeight % 8 != 0 ? (oldHeight/8)*8 + 8  : oldHeight;
        }
    }
}

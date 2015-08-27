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

namespace Rainbow.ImgLib.Common
{
    public static class ImageUtils
    {
        public static int GetMipmapWidth(int width, int mipmapIndex)
        {
            return width / (1 << mipmapIndex);
        }

        public static int GetMipmapHeight(int height, int mipmapIndex)
        {
            return height / (1 << mipmapIndex);
        }

        public static int Conv5To8(int value)
        {
            return (value & 0x1F) * 8;
        }

        public static int Conv6To8(int value)
        {
            return (value & 0x3F) * 4;
        }

        public static int Conv4To8(int value)
        {
            return (value & 0xF) * 0x11;
        }

        public static int Conv3To8(int value)
        {
            return (value & 0x7) * 32;
        }

        public static int Conv8To5(int value)
        {
            return (value & 0xFF) >> 3;
        }

        public static int Conv8To6(int value)
        {
            return (value & 0xFF) >> 2;
        }

        public static int Conv8To4(int value)
        {
            return (value & 0xFF) >> 4;
        }

        public static int Conv8To3(int value)
        {
            return (value & 0xFF) >> 5;
        }
    }
}

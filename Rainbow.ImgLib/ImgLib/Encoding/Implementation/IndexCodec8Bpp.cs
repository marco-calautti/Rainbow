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

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class IndexCodec8Bpp : IndexCodec
    {
        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            return pixelData[x + y * width];
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            byte[] packed = new byte[length];
            for (int i = start; i < length; i++)
            {
                if (indexes[i] > 255)
                {
                    throw new ArgumentException("Too big index!");
                }
                packed[i - start] = (byte)indexes[i];
            }
            return packed;
        }

        public override int BitDepth
        {
            get { return 8; }
        }
    }
}

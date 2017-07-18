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

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class IndexCodec4Bpp : IndexCodecEndiannessDependent
    {
        public IndexCodec4Bpp(ByteOrder order):
            base(order) { }

        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            int pos = x + y * width;
            byte b = pixelData[pos / 2];

            if (ByteOrder == ByteOrder.LittleEndian)
            {
                return pos % 2 == 0 ? b & 0xF : (b >> 4) & 0xF;
            }
            else
            {
                return pos % 2 != 0 ? b & 0xF : (b >> 4) & 0xF;
            }
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            if (length % 2 != 0)
                throw new ArgumentException("Number of indexes must be odd!");

            byte[] packed = new byte[length / 2];
            int k = 0;
            for (int i = start; i < length; i += 2)
            {
                if (indexes[i] > 15 || indexes[i + 1] > 15)
                {
                    throw new ArgumentException("Too big index!");

                }

                if (ByteOrder == ByteOrder.LittleEndian)
                {
                    packed[k++] = (byte)((indexes[i + 1] << 4) | indexes[i]);
                }
                else
                {
                    packed[k++] = (byte)((indexes[i] << 4) | indexes[i + 1]);
                }
            }
            return packed;
        }

        public override int BitDepth
        {
            get { return 4; }
        }
    }
}

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

namespace Rainbow.ImgLib.Filters
{
    public class TileFilter : ImageFilter
    {
        private readonly int bpp;
        private readonly int tileWidth;
        private readonly int tileHeight;
        private readonly int width, height;

        public TileFilter(int bpp, int tileWidth, int tileHeight, int width, int height)
        {
            this.bpp = bpp;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.width = width;
            this.height = height;
            TileDimensionsAsBytes = false;
        }

        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            int encodingWidth = GetWidthForEncoding(this.width);
            int encodingHeight = GetHeightForEncoding(this.height);

            int w = (encodingWidth * bpp) / 8;
            byte[] Buf = new byte[w*encodingHeight];

            int lineSize = TileDimensionsAsBytes? tileWidth : (tileWidth * bpp) / 8;

            int origW = (this.width * bpp) / 8;
            int i = 0;

            for (int y = 0; y < encodingHeight; y += tileHeight)
            {
                for (int x = 0; x < w; x += lineSize)
                {
                    for (int tileY = y; tileY < y + tileHeight; tileY++)
                    {
                        for (int tileX = x; tileX < x + lineSize; tileX++)
                        {
                            byte data = 0;
                            if (tileX < origW && tileY < height)
                            {
                                data = originalData[index + tileY * w + tileX];
                            }

                            Buf[i++] = data;
                        }
                    }
                }
            }

            return Buf;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] Buf = new byte[length];
            int w = (this.width * bpp) / 8;

            int lineSize = TileDimensionsAsBytes ? tileWidth : (tileWidth * bpp) / 8;

            int i = 0;

            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < w; x += lineSize)
                {
                    for (int tileY = y; tileY < y + tileHeight; tileY++)
                    {
                        for (int tileX = x; tileX < x + lineSize; tileX++)
                        {
                            byte data = originalData[index + i++];

                            if (tileX >= w || tileY >= height)
                            {
                                continue;
                            }

                            Buf[tileY * w + tileX] = data;
                        }
                    }
                }
            }

            return Buf;
        }

        public bool TileDimensionsAsBytes
        {
            get;
            set;
        }

        public override int GetWidthForEncoding(int realWidth)
        {
            int encodingWidth = realWidth;

            int w = (realWidth * bpp) / 8;

            int lineSize = TileDimensionsAsBytes ? tileWidth : (tileWidth * bpp) / 8;

            if (w % lineSize != 0)
            {
                encodingWidth += ((lineSize - w % lineSize) * 8) / bpp;
            }

            return encodingWidth;

        }
        public override int GetHeightForEncoding(int realHeight)
        {
            int encodingHeight = realHeight;

            if (height % tileHeight != 0)
            {
                encodingHeight += (height - height % tileHeight);
            }

            return encodingHeight;
        }
    }
}

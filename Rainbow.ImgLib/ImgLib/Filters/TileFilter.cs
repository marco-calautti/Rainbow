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
        private int bpp;
        private int tileWidth;
        private int tileHeight;
        private int width, height;

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
            /*byte[] newData = new byte[length];

            int lineSize = (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;
            int pitch = (width * bpp) / 8;

            int tile = 0;

            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < pitch; x += lineSize)
                {
                    for (int line = 0; line < tileHeight; line++)
                    {
                        Array.Copy(originalData, pitch * (y + line) + x, newData, index + tile * tileSize + line * lineSize, lineSize);
                    }

                    tile++;
                }
            }

            return newData;*/

            byte[] Buf = new byte[length];
            int w = (this.width * bpp) / 8;

            int lineSize = TileDimensionsAsBytes? tileWidth : (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;

            int rowblocks = w / lineSize;

            int totalBlocksx = w / lineSize;
            int totalBlocksy = height / tileHeight;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * tileSize;

                    for (int y = 0; y < tileHeight; y++)
                    {
                        int absolutey = y + blocky * tileHeight;
                        Array.Copy(originalData, index + blockx * lineSize + absolutey * w , Buf, + block_address + y * lineSize, lineSize);
                    }
                }

            int start = totalBlocksy * rowblocks * lineSize * tileHeight;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            /*byte[] newData = new byte[length];

            int lineSize = (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;
            int pitch = (width * bpp) / 8;

            int tile = 0;

            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < pitch; x += lineSize)
                {
                    for (int line = 0; line < tileHeight; line++)
                    {
                        Array.Copy(originalData, index + tile * tileSize + line * lineSize, newData, pitch * (y + line) + x, lineSize);
                    }

                    tile++;
                }
            }

            return newData;*/

            byte[] Buf = new byte[length];
            int w = (this.width * bpp) / 8;
            int lineSize = TileDimensionsAsBytes ? tileWidth : (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;

            int rowblocks = w / lineSize;

            int totalBlocksx = w / lineSize;
            int totalBlocksy = height / tileHeight;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * tileSize;

                    for (int y = 0; y < tileHeight; y++)
                    {
                        int absolutey = y + blocky * tileHeight;
                        Array.Copy(originalData, index + block_address + y * lineSize, Buf, blockx * lineSize + absolutey * w, lineSize);
                    }
                }

            int start = totalBlocksy * rowblocks * lineSize * tileHeight;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }

        public bool TileDimensionsAsBytes
        {
            get;
            set;
        }
    }
}

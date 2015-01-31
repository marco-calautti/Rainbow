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
    public class SwizzleFilter : ImageFilter
    {
        private int width,height,bitDepth;

        public SwizzleFilter(int width,int height,int bitDepth)
        {
            this.width=width;
            this.height = height;
            this.bitDepth = bitDepth;
        }


        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            byte[] Buf = new byte[length];
            int val = 16;
            int w = (this.width * bitDepth) / 8;
            int rowblocks = w / val;

            int totalBlocksx = w / val;
            int totalBlocksy = height / 8;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;
                            
                            Buf[block_address + x + y * val] =
                                originalData[index + absolutex + absolutey * w];
                            
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] Buf = new byte[length];
            int val = 16;
            int w = (this.width * bitDepth) / 8;
            int rowblocks = w / val;

            int totalBlocksx = w / val;
            int totalBlocksy = height / 8;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;
                           
                            Buf[absolutex + absolutey * w] =
                                  originalData[index + block_address + x + y * val];
                            
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }
    }
}

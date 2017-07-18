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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class TIM2PaletteFilter : PaletteFilter
    {
        private readonly int bitDepth;
        public TIM2PaletteFilter(int bitDepth)
        {
            this.bitDepth = bitDepth;
        }

        public override Color[] Defilter(Color[] originalData, int index, int length)
        {
            Color[] newColors = new Color[length];

            if(bitDepth!=8)
            {
                Array.Copy(originalData, index, newColors, 0, length);
                return newColors;
            }

            int parts = length / 32;
            int stripes=2;
            int colors = 8;
            int blocks = 2;

            int i = 0;
            for (int part = 0; part < parts; part++)
            {
                for (int block = 0; block < blocks; block++)
                {
                    for (int stripe = 0; stripe < stripes; stripe++)
                    {

                        for (int color = 0; color < colors; color++)
                        {
                            newColors[i++] = originalData[ index + part * colors * stripes * blocks + block * colors + stripe * stripes * colors + color ];
                        }
                    }
                }
            }

            return newColors;
        }

        public override Color[] ApplyFilter(Color[] originalData, int index, int length)
        {
            return Defilter(originalData, index, length);
        }
    }
}

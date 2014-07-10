using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class TIM2PaletteFilter : Filter<Color>
    {
        private int bitDepth;
        public TIM2PaletteFilter(int bitDepth)
        {
            this.bitDepth = bitDepth;
        }

        public override Color[] ApplyFilter(Color[] originalData, int index, int length)
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

        public override Color[] Defilter(Color[] originalData, int index, int length)
        {
            return ApplyFilter(originalData, index, length);
        }
    }
}

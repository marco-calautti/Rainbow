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

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodec24BitRGB : ColorCodec
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 3; i++)
            {
                pal.Add(Color.FromArgb(255, palette[start + i * 3], palette[start + i * 3 + 1], palette[start + i * 3 + 2]));
            }

            return pal.ToArray();
        }

        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {
            byte[] palette = new byte[colors.Length * 3];
            for (int i = start; i < colors.Length; i++)
            {
                palette[(i - start) * 3] = colors[i].R;
                palette[(i - start) * 3 + 1] = colors[i].G;
                palette[(i - start) * 3 + 2] = colors[i].B;
            }
            return palette;
        }

        public override int BitDepth { get { return 24; } }
    }
}

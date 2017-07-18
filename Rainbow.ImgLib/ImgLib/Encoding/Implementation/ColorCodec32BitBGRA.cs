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
    /// <summary>
    /// This  ColorDecoder decodes sequences of pixels in 32 bit BGRA format.
    /// </summary>
    public class ColorCodec32BitBGRA : ColorCodec
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
            {
                pal.Add(Color.FromArgb(palette[start + i * 4 + 3], palette[start + i * 4 + 2], palette[start + i * 4 + 1], palette[start + i * 4]));
            }

            return pal.ToArray();
        }

        public override int BitDepth { get { return 32; } }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            byte[] encoded = new byte[length * 4];

            for (int i = 0; i < encoded.Length; i += 4)
            {
                encoded[i] = colors[start + i / 4].B;
                encoded[i + 1] = colors[start + i / 4].G;
                encoded[i + 2] = colors[start + i / 4].R;
                encoded[i + 3] = colors[start + i / 4].A;
            }

            return encoded;
        }
    }
}

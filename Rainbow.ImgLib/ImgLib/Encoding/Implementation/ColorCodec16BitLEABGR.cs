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

using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodec16BitLEABGR : ColorCodec
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            BinaryReader reader = new BinaryReader(new MemoryStream(palette, start, size));
            for (int i = 0; i < size / 2; i++)
            {
                ushort data = reader.ReadUInt16();

                int red = data & 0x1F;
                data >>= 5;
                int green = data & 0x1F;
                data >>= 5;
                int blue = data & 0x1F;
                data >>= 5;
                int alpha = data == 0 ? 0 : 255;

                pal.Add(Color.FromArgb(alpha, ImageUtils.Conv5To8(red), ImageUtils.Conv5To8(green), ImageUtils.Conv5To8(blue)));
            }

            reader.Close();
            return pal.ToArray();
        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            byte[] palette = new byte[colors.Length * 2];

            for (int i = start; i < colors.Length; i++)
            {
                ushort data = (ushort)(colors[i].A > 127 ? 0x8000 : 0);

                data |= (ushort)((ImageUtils.Conv8To5(colors[i].B) << 10) | (ImageUtils.Conv8To5(colors[i].G) << 5) | (ImageUtils.Conv8To5(colors[i].R) & 0x1F));
                palette[(i - start) * 2] = (byte)(data & 0xFF);
                palette[(i - start) * 2 + 1] = (byte)(data >> 8);
            }
            return palette;
        }

        public override int BitDepth { get { return 16; } }
    }
}

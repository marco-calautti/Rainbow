//Copyright (C) 2014 Marco (Phoenix) Calautti.

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

using System.Drawing;

namespace Rainbow.ImgLib.Encoding
{

    public class ColorEncoder32BitRGBA : ColorEncoder
    {
        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {

            byte[] palette = new byte[colors.Length * 4];
            for (int i = start; i < colors.Length; i++)
            {
                palette[(i - start) * 4] = colors[i].R;
                palette[(i - start) * 4 + 1] = colors[i].G;
                palette[(i - start) * 4 + 2] = colors[i].B;
                palette[(i - start) * 4 + 3] = colors[i].A;
            }
            return palette;
        }

        public override int BitDepth
        {
            get { return 32; }
        }
    }
    public class ColorEncoder24BitRGB : ColorEncoder
    {
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

        public override int BitDepth
        {
            get { return 24; }
        }
    }

    public class ColorEncoder16BitLEABGR : ColorEncoder
    {
        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            byte[] palette = new byte[colors.Length * 2];

            for (int i = start; i < colors.Length; i++)
            {
                ushort data = (ushort)(colors[i].A > 127 ? 0x8000 : 0);

                data |= (ushort)(((colors[i].B >> 3) << 10) | ((colors[i].G >> 3) << 5) | ((colors[i].R >> 3) & 0x1F));
                palette[(i - start) * 2] = (byte)(data & 0xFF);
                palette[(i - start) * 2 + 1] = (byte)(data >> 8);
            }
            return palette;
        }

        public override int BitDepth
        {
            get { return 16; }
        }
    }
}

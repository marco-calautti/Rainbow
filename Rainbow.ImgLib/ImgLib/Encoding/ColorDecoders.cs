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

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding
{
    /// <summary>
    /// This  ColorDecoder decodes sequences of pixels in 32 bit RGBA format.
    /// </summary>
    public class ColorDecoder32BitRGBA : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
                pal.Add(Color.FromArgb(palette[start + i * 4 + 3], palette[start + i * 4], palette[start + i * 4 + 1], palette[start + i * 4 + 2]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 32; } }
    }

    /// <summary>
    /// This  ColorDecoder decodes sequences of pixels in 32 bit BGRA format.
    /// </summary>
    public class ColorDecoder32BitBGRA : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
                pal.Add(Color.FromArgb(palette[start + i * 4 + 3], palette[start + i * 4 + 2], palette[start + i * 4 + 1], palette[start + i * 4]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 32; } }
    }

    public class ColorDecoder24BitRGB : ColorDecoder
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 3; i++)
                pal.Add(Color.FromArgb(255, palette[start + i * 3], palette[start + i * 3 + 1], palette[start + i * 3 + 2]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 24; } }
    }

    public class ColorDecoder16BitLEABGR : ColorDecoder
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
                int alpha = data == 0 ? 0 : 255;

                pal.Add(Color.FromArgb(alpha, red * 8, green * 8, blue * 8));
            }

            return pal.ToArray();
        }

        public override int BitDepth { get { return 16; } }
    }

    public class ColorDecoderRGB565 : ColorDecoder
    {

        public ColorDecoderRGB565(ByteOrder order)
        {
            ByteOrder = order;
        }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length/2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                if (ByteOrder == ByteOrder.BigEndian)
                    color = reader.ReadUInt16BE();
                else
                    color = reader.ReadUInt16();

                int red, green, blue;
                red = ((color >> 11) & 0x1f) * 8;
                green = ((color >> 5) & 0x3f) * 4;
                blue = ((color) & 0x1f) * 8;

                encoded[i] = Color.FromArgb(255, red, green, blue);
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }

        public ByteOrder ByteOrder
        {
            get;
            private set;
        }
    }

    public class ColorDecoderRGB5A3 : ColorDecoder
    {
        public ColorDecoderRGB5A3(ByteOrder order)
        {
            ByteOrder = order;
        }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length/2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                if (ByteOrder == ByteOrder.BigEndian)
                    color = reader.ReadUInt16BE();
                else
                    color = reader.ReadUInt16();

                int red, green, blue, alpha;
                if ((color & 0x8000) != 0) //no alpha
                {
                    red = ((color >> 10) & 0x1F) * 8;
                    green = ((color >> 5) & 0x1F) * 8;
                    blue = ((color) & 0x1F) * 8;
                    alpha = 255;
                }
                else // with alpha
                {
                    alpha = ((color >> 12) & 0x7) * 32;
                    red = ((color >> 8) & 0xf) * 16;
                    green = ((color >> 4) & 0xf) * 16;
                    blue = ((color) & 0xf) * 16;
                }

                encoded[i] = Color.FromArgb(alpha, red, green, blue);
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }

        public ByteOrder ByteOrder
        {
            get;
            private set;
        }
    }
}
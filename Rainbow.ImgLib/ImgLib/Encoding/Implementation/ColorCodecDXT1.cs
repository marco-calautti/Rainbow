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
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;
using System.Drawing;
using Rainbow.ImgLib.Filters;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecDXT1 : ColorCodecEndiannessDependent
    {
        private readonly int width, height;
        private static Color[] clut = new Color[4];

        public ColorCodecDXT1(ByteOrder order, int width, int height):
            base(order)
        {
            this.width = width;
            this.height = height;
        }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            ImageFilter filter = GetImageFilter();

            BinaryReader reader=null;
            if (filter != null)
            {
                byte[] data = filter.Defilter(colors, start, length);
                reader = new BinaryReader(new MemoryStream(data));
            }
            else
            {
                reader = new BinaryReader(new MemoryStream(colors, start, length));
            }

            Color[] decoded = new Color[FullWidth * FullHeight];
            Color[] tile = new Color[4 * 4];

            for (int y = 0; y < FullHeight; y += 4)
            {
                for (int x = 0; x < FullWidth; x += 4)
                {
                    DecodeDXT1Block(reader, tile);
                    for (int line = 0; line < 4; line++)
                    {
                        Array.Copy(tile, line * 4, decoded, FullWidth * (y + line) + x, 4);

                    }
                }
            }

            reader.Close();

            if (FullWidth == width && FullHeight == height)
            {
                return decoded;
            }

            Color[] decodedRealSize = new Color[width * height];

            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    decodedRealSize[k++] = decoded[y * FullWidth + x];
                }
            }

            return decodedRealSize;

        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }

        public override int BitDepth
        {
            get { return 4; }
        }

        public int FullWidth
        {
            get { return GetFullWidth(width); }
        }

        public int FullHeight
        {
            get { return GetFullHeight(height); }
        }

        public override int GetBytesNeededForEncode(int width, int height, ImageFilter referenceFilter = null)
        {
            return GetFullWidth(width) * GetFullHeight(height) * BitDepth / 8;
        }

        protected void DecodeDXT1Block(BinaryReader reader, Color[] tile)
        {

            ushort color1, color2;
            byte[] table;
            color1 = reader.ReadUInt16(ByteOrder);
            color2 = reader.ReadUInt16(ByteOrder);

            table = reader.ReadBytes(4);

            int blue1 = ImageUtils.Conv5To8(color1 & 0x1F);
            int blue2 = ImageUtils.Conv5To8(color2 & 0x1F);
            int green1 = ImageUtils.Conv6To8((color1 >> 5) & 0x3F);
            int green2 = ImageUtils.Conv6To8((color2 >> 5) & 0x3F);
            int red1 = ImageUtils.Conv5To8((color1 >> 11) & 0x1F);
            int red2 = ImageUtils.Conv5To8((color2 >> 11) & 0x1F);

	        clut[0] = Color.FromArgb(255,red1, green1, blue1);
            clut[1] = Color.FromArgb(255, red2, green2, blue2);

            if (color1 > color2)
            {
                int blue3 = (2 * blue1 + blue2) / 3;
                int green3 = (2 * green1 + green2) / 3;
                int red3 = (2 * red1 + red2) / 3;

                int blue4 = (2 * blue2 + blue1) / 3;
                int green4 = (2 * green2 + green1) / 3;
                int red4 = (2 * red2 + red1) / 3;

                clut[2] = Color.FromArgb(255, red3,green3,blue3);
                clut[3] = Color.FromArgb(255,red4,green4,blue4);
            }
            else
            {
                clut[2] = Color.FromArgb(255, (red1 + red2)  / 2, // Average
                                              (green1 + green2) / 2,
                                              (blue1 + blue2) / 2);
                clut[3] = Color.FromArgb(0, 0, 0, 0);  // Color2 but transparent
            }

            int k=0;
            for (int y = 0; y < 4; y++)
            {
                int val = table[y];
                for (int x = 0; x < 4; x++)
                {
                    tile[k++] = clut[(val >> 6) & 3];
                    val <<= 2;
                }
            }
        }

        protected virtual ImageFilter GetImageFilter()
        {
            return null;
        }

        protected virtual int GetFullWidth(int width)
        {
            return width % 4 != 0 ? (width / 4) * 4 + 4 : width;
        }

        protected virtual int GetFullHeight(int height)
        {
            return height % 4 != 0 ? (height / 4) * 4 + 4 : height;
        }
    }
}

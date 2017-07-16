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
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecRGB5A3 : ColorCodecEndiannessDependent
    {
        public ColorCodecRGB5A3(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length / 2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                color = reader.ReadUInt16(ByteOrder);


                int red, green, blue, alpha;
                if ((color & 0x8000) != 0) //no alpha
                {
                    red = ImageUtils.Conv5To8((color >> 10) & 0x1F);
                    green = ImageUtils.Conv5To8((color >> 5) & 0x1F);
                    blue = ImageUtils.Conv5To8((color) & 0x1F);
                    alpha = 255;
                }
                else // with alpha
                {
                    alpha = ImageUtils.Conv3To8((color >> 12) & 0x7);
                    red = ImageUtils.Conv4To8((color >> 8) & 0xf);
                    green = ImageUtils.Conv4To8((color >> 4) & 0xf);
                    blue = ImageUtils.Conv4To8((color) & 0xf);
                }

                encoded[i] = Color.FromArgb(alpha, red, green, blue);
            }
            reader.Close();
            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            byte[] encoded = new byte[length * 2];

            for(int i=0;i<length;i++)
            {
                ushort color = 0;
                int red, green, blue;

                int alpha = ImageUtils.Conv8To3(colors[start + i].A);
                if (alpha == 7) //we don't need alpha
                {
                    red = ImageUtils.Conv8To5(colors[start + i].R);
                    green = ImageUtils.Conv8To5(colors[start + i].G);
                    blue = ImageUtils.Conv8To5(colors[start + i].B);
                    color = (ushort)(((red & 0x1F) << 10) | ((green & 0x1F) << 5) | blue & 0x1F);
                    color |= 0x8000; //mark as no alpha
                }
                else
                {
                    red = ImageUtils.Conv8To4(colors[start + i].R);
                    green = ImageUtils.Conv8To4(colors[start + i].G);
                    blue = ImageUtils.Conv8To4(colors[start + i].B);
                    color = (ushort)( ((alpha & 0x7) << 12) | ((red & 0xF) << 8) | ((green & 0xF) << 4) | blue & 0xF);
                }

                encoded[i * 2] = (byte)(ByteOrder == ByteOrder.LittleEndian ? color & 0xFF : (color >> 8) & 0xFF);
                encoded[i * 2 + 1] = (byte)(ByteOrder == ByteOrder.LittleEndian ? (color >> 8) & 0xFF : color & 0xFF);
            }

            return encoded;
        }
    }
}

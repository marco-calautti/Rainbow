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
    public class ColorCodecRGB565 : ColorCodecEndiannessDependent
    {

        public ColorCodecRGB565(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length / 2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                color = reader.ReadUInt16(ByteOrder);


                int red, green, blue;
                red = ImageUtils.Conv5To8((color >> 11) & 0x1f);
                green = ImageUtils.Conv6To8((color >> 5) & 0x3f);
                blue = ImageUtils.Conv5To8((color) & 0x1f);

                encoded[i] = Color.FromArgb(255, red, green, blue);
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
                int red = ImageUtils.Conv8To5(colors[start + i].R);
                int green = ImageUtils.Conv8To6(colors[start + i].G);
                int blue = ImageUtils.Conv8To5(colors[start + i].B);

                ushort color = (ushort)(((red & 0x1F) << 11) | ((green & 0x3F) << 5) | (blue & 0x1F));
                encoded[i * 2] = (byte)(ByteOrder == ByteOrder.LittleEndian ? color & 0xFF : (color >> 8) & 0xFF);
                encoded[i * 2 + 1] = (byte)(ByteOrder == ByteOrder.LittleEndian ? (color >> 8) & 0xFF : color & 0xFF);
            }

            return encoded;
        }
    }
}

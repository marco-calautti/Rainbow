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
    public class ColorCodecIA4: ColorCodecEndiannessDependent
    {
        public ColorCodecIA4(ByteOrder order) :
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));
            Color[] decoded = new Color[length];

            for(int i=0;i<decoded.Length;i++)
            {
                byte data = reader.ReadByte();

                int alpha = ByteOrder == ByteOrder.LittleEndian ? data & 0xF : (data >> 4) & 0xF;
                int intensity = ByteOrder == ByteOrder.LittleEndian ? (data>>4) & 0xF : data & 0xF;

                alpha = ImageUtils.Conv4To8(alpha);
                intensity = ImageUtils.Conv4To8(intensity);
                decoded[i] = Color.FromArgb(alpha, intensity, intensity, intensity);
            }
            reader.Close();
            return decoded;
        }

        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {
            byte[] encoded = new byte[length];

            for(int i=0; i<length; i++)
            {
                Color gray = ImageUtils.ToGrayScale(colors[start + i]);

                int alphaNibble = ImageUtils.Conv8To4(gray.A);
                int intensityNibble = ImageUtils.Conv8To4(gray.R);

                byte value = 0;
                if(ByteOrder == ByteOrder.LittleEndian)
                {
                    value = (byte)((alphaNibble & 0xF) | ((intensityNibble & 0xF) << 4));
                }else
                {
                    value = (byte)((intensityNibble & 0xF) | ((alphaNibble & 0xF) << 4));
                }

                encoded[i] = value;
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 8; }
        }
    }
}
